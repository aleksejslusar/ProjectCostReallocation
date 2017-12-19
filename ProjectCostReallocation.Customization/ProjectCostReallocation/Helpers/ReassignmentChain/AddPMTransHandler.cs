using System;
using System.Collections.Generic;
using System.Linq;
using PX.Data;
using PX.Objects.Common.Extensions;
using PX.Objects.PM;

namespace ProjectCostReallocation
{
    public class AddPMTransHandler: TransactionEntityBase
    {
        #region .ctor

        public AddPMTransHandler(ProjectCostReassignmentEntry reassignmentEntry, RegisterEntry registerEntry, PMCostReassigmentEntity entity) : base(reassignmentEntry, registerEntry, entity)
        {
        }

        #endregion

        #region Overrides

        protected override void HandlerRequest()
        {
            var reassignmentSource = ReassignmentEntry.PMCostReassignmentSource.Search<UsrPMCostReassignmentSource.lineID>(Entity.SourceLineID);
            var reassigmentDestination = ReassignmentEntry.PMCostReassignmentDestination.Search<UsrPMCostReassignmentDestination.lineID>(Entity.DestinaitonLineID);
            var sourcePMTrans = GetSourсeTransaction(reassignmentSource);

            //Validate if no any source transactions to reassign
            if (!sourcePMTrans.Any())
            {
                Entity.ProcessingWarning = true;
                Entity.ProcessingMessage = PMCostReassignmentMessages.REASSAIMENT_NOTHING_TO_REASSIGN_TRAN_NOT_FOUND;
                return;
            }

            //Validate if entityPair was reassigned earlier
            var canReassignmenPMTrans = new List<PMTran>();    
            var allProcessedDestTaskIds = new HashSet<int?>();
            var currentReassignment = ReassignmentEntry.PMCostReassignment.Current;
            decimal sourcePMTranBalance = 0;
            if (currentReassignment != null)
            {
                var reassigmentDestinaitonTaskIds = ReassignmentEntry.PMCostReassignmentDestination.Select()
                                                                                                   .Select(d => d.GetItem<PMTask>())
                                                                                                   .Where(t => t.Status != ProjectTaskStatus.Completed)
                                                                                                   .Select(t => t.TaskID);
                foreach (var sourcePMTran in sourcePMTrans)
                {
                    var processedDestTaskIds = ReassignmentEntry.ProcessedTransactions.Select(sourcePMTran.TranID, currentReassignment.PMReassignmentID, currentReassignment.RevID)
                                                                                      .Select(i => i.GetItem<UsrPMCostReassignmentRunHistory>().DestinationTaskID)
                                                                                      .ToList();

                    sourcePMTranBalance += CalculateBalanceSourceAmount(sourcePMTran); 
                    allProcessedDestTaskIds.AddRange(processedDestTaskIds);
                    if (!processedDestTaskIds.Any() || !processedDestTaskIds.Contains(Entity.DestinationTaskID))
                    {
                        canReassignmenPMTrans.Add(sourcePMTran);
                    }
                }

                if (!canReassignmenPMTrans.Any())
                {
                    string processingMessage;
                    //Availability check not reassigned reassignments in reassignmet. If available - processing message text from first condition, otherwise - from second 
                    if (reassigmentDestinaitonTaskIds.Except(allProcessedDestTaskIds).Any())
                    {
                        processingMessage = PMCostReassignmentMessages.REASSAIMENT_NOTHING_TO_REASSIGN_TRAN_REQASSIGNED_EARLIER;
                    }
                    else
                    {
                        //Check availability for balance more than zero in source transaction. Depending on the condition, the message changes
                        processingMessage = Math.Abs(sourcePMTranBalance) > 0 ? string.Format(PMCostReassignmentMessages.REASSAIMENT_NOTHING_TO_REASSIGN_TRAN_REQASSIGNED_EARLIER_BUT_SOURCE_BALANCE_MORE_THAN_ZERO, currentReassignment.PMReassignmentID)
                                                                              : PMCostReassignmentMessages.REASSAIMENT_NOTHING_TO_REASSIGN_TRAN_REQASSIGNED_EARLIER;
                    }
                    
                    Entity.ProcessingWarning = true;
                    Entity.ProcessingMessage = processingMessage;
                    return;
                }
            }

            

            //Create transactions
            foreach (var sourcePMTran in canReassignmenPMTrans)
            {
                var balansedSourceAmount = CalculateBalanceSourceAmount(sourcePMTran);
                var reassignmentData = CalculateReassignmentAmount(sourcePMTran, balansedSourceAmount, reassigmentDestination);
                
                //Valudations
                if (!reassignmentData.IsValid)
                {
                    Entity.ProcessingWarning = true;
                    Entity.ProcessingMessage = PMCostReassignmentMessages.REASSAIMENT_AMOUNT_ERROR;
                    return;
                }

                //Write data
                WriteTransactions(Entity, reassignmentData, sourcePMTran, reassignmentSource, reassigmentDestination);

                //Write processed trans for show processing results
                if (Math.Abs(balansedSourceAmount) <= Math.Abs(reassignmentData.Amount))
                {
                    ReassignedSourcePMTrans.Add(sourcePMTran);
                }
                
            }
            Entity.ProcessingSuccess = true;
            Entity.ProcessingMessage = PMCostReassignmentMessages.REASSIGNMENT_SUCCESS;
        }

        #endregion

        #region Private

        private List<PMTran> GetSourсeTransaction(UsrPMCostReassignmentSource reassignmentSource)
        {
            /* Select logic explanation:
             * For each row on the Run Reassignment Cost screen select PM transactions where From Project, 
             * From Task and Account Group From/To values (use range is From and To are specified) and 
             * PMTran.usrReassigned = 1.*/

            var pmTran = new PXSelect<PMTran, Where<PMTran.projectID, Equal<Required<PMTran.projectID>>,
                                                And<PMTran.taskID, Equal<Required<PMTran.taskID>>,
                                                And<PMTran.released, Equal<True>, 
                                                And<PMTranExt.usrReassigned, Equal<False>>>>>>(RegisterEntry);

            if (reassignmentSource.AccountGroupFrom != null && reassignmentSource.AccountGroupTo != null)
            {
                pmTran.WhereAnd<Where<PMTran.accountGroupID, Between<Required<PMTran.accountGroupID>, Required<PMTran.accountGroupID>>>>();
            }

            if ((reassignmentSource.AccountGroupFrom != null && reassignmentSource.AccountGroupTo == null) || (reassignmentSource.AccountGroupFrom == null && reassignmentSource.AccountGroupTo != null))
            {
                pmTran.WhereAnd<Where<PMTran.accountGroupID, Equal<Required<PMTran.accountGroupID>>>>();
            }


            var accountGroupFrom = Math.Min(reassignmentSource.AccountGroupFrom.GetValueOrDefault(), reassignmentSource.AccountGroupTo.GetValueOrDefault());
            var accountGroupTo = Math.Max(reassignmentSource.AccountGroupFrom.GetValueOrDefault(), reassignmentSource.AccountGroupTo.GetValueOrDefault());

            return pmTran.Select(reassignmentSource.ProjectID, reassignmentSource.TaskID, accountGroupFrom, accountGroupTo)
                         .Select(e => (PMTran)e)
                         .ToList();
        }

        private decimal CalculateBalanceSourceAmount(PMTran sourcePMTran)
        {
            var currentReassignment = ReassignmentEntry.PMCostReassignment.Current;
            if (currentReassignment != null)
            {
                var existsTransAmount = Math.Abs(ReassignmentEntry.AllProcessedTransactions.Select(sourcePMTran.TranID, currentReassignment.PMReassignmentID)
                                                                                           .Select(i => i.GetItem<PMTran>())
                                                                                           .Where(i => Math.Abs(i.Amount.GetValueOrDefault()) > 0)
                                                                                           .Sum(i => i.Amount.GetValueOrDefault()));

                var sourceTransAmount = sourcePMTran.Amount.GetValueOrDefault();               
                var result = sourcePMTran.Amount > 0 ? decimal.Subtract(sourceTransAmount, existsTransAmount) : decimal.Add(sourceTransAmount, existsTransAmount);
                return result;
            }

            return 0;
        }

        private ReassigmentValue CalculateReassignmentAmount(PMTran sourcePMTran, decimal balansedSourceAmount, UsrPMCostReassignmentDestination reassignmentDestination)
        {
            decimal sourceAmount = 0;
            decimal destAmount = 0;

            var totalReassignmentValue1 = ReassignmentEntry.PMCostReassignment.Current.ReassignmentValue1Total;
            var totalReassignmentValue2 = ReassignmentEntry.PMCostReassignment.Current.ReassignmentValue2Total;
            var reassigmentSelection = reassignmentDestination.ReassignmentSelection;

            var currentReassignment = ReassignmentEntry.PMCostReassignment.Current;
            if (currentReassignment != null)
            {
                var processedDestTaskIds = ReassignmentEntry.ProcessedTransactions.Select(sourcePMTran.TranID, currentReassignment.PMReassignmentID, currentReassignment.RevID)
                                                                                  .Select(i => i.GetItem<UsrPMCostReassignmentRunHistory>().DestinationTaskID)
                                                                                  .ToList();
                if (processedDestTaskIds.Any())
                {
                    foreach (UsrPMCostReassignmentDestination result in ReassignmentEntry.PMCostReassignmentDestination.Select())
                    {                        
                        if (result != null && processedDestTaskIds.Contains(result.TaskID))
                        {
                            totalReassignmentValue1 -= result.ReassignmentValue1;
                            totalReassignmentValue2 -= result.ReassignmentValue2;
                        }
                    }
                }
            }
            
            switch (reassigmentSelection)
            {
                case ReassignmentSelectionAttribute.Values.UnitCount: //Number of Units
                    {
                        sourceAmount = totalReassignmentValue2.GetValueOrDefault();
                        destAmount = reassignmentDestination.ReassignmentValue2.GetValueOrDefault();
                        if (destAmount == 0 || sourceAmount == 0)
                        {
                            throw new Exception(PMCostReassignmentMessages.REASSAIMENT_AMOUNT_ERROR_UNITS);
                        }
                    }
                    break;
                case ReassignmentSelectionAttribute.Values.SquareFootage: //Square Footage
                    {
                        sourceAmount = totalReassignmentValue1.GetValueOrDefault();
                        destAmount = reassignmentDestination.ReassignmentValue1.GetValueOrDefault();
                        if (destAmount == 0 || sourceAmount == 0)
                        {
                            throw new Exception(PMCostReassignmentMessages.REASSAIMENT_AMOUNT_ERROR_SF);
                        }
                    }
                    break;
            }

            var percentage = decimal.Divide(destAmount, sourceAmount);
            var amount = decimal.Multiply(balansedSourceAmount, percentage);

            var isValid = percentage > 0 && percentage <= 1 && amount != 0;
            return new ReassigmentValue(isValid, amount, sourceAmount, destAmount, percentage, reassigmentSelection);
        }

        private void WriteTransactions(PMCostReassigmentEntity entityPair, ReassigmentValue reassignmentValue, PMTran sourcePmTran, UsrPMCostReassignmentSource reassignmentSource, UsrPMCostReassignmentDestination reassignmentDestination)
        {
            PMSetup setupRow = ReassignmentEntry.PMSetupSelect.Select().FirstOrDefault();

            var setupOffsetAccountID = PXCache<PMSetup>.GetExtension<PMSetupExt>(setupRow).UsrReassignmentAccountID;
            var setupOffsetAccountSubID = PXCache<PMSetup>.GetExtension<PMSetupExt>(setupRow).UsrReassignmentSubID;

            //Create Transactions
            var reassignmentSourceTran = new PMTran
            {
                RefNbr = RegisterEntry.Document.Current.RefNbr,
                BranchID = sourcePmTran.BranchID,
                ProjectID = reassignmentSource.ProjectID,
                TaskID = reassignmentSource.TaskID,
                AccountGroupID = sourcePmTran.AccountGroupID,
                ResourceID = sourcePmTran.ResourceID,
                BAccountID = sourcePmTran.BAccountID,
                LocationID = sourcePmTran.LocationID,
                InventoryID = sourcePmTran.InventoryID,
                Description = sourcePmTran.Description,
                UOM = sourcePmTran.UOM,
                Qty = sourcePmTran.Qty,
                Billable = sourcePmTran.Billable,
                BillableQty = sourcePmTran.BillableQty,
                UnitRate = sourcePmTran.UnitRate,
                Amount = (reassignmentValue.Amount * -1),
                AccountID = sourcePmTran.AccountID,
                SubID = sourcePmTran.SubID,
                OffsetAccountID = sourcePmTran.OffsetAccountID ?? setupOffsetAccountID,
                OffsetSubID = sourcePmTran.OffsetSubID ?? setupOffsetAccountSubID,
                Date = entityPair.ReassignmentDate,
                FinPeriodID = entityPair.ReassignmentFinPeriodID,
                BatchNbr = null,
                EarningType = sourcePmTran.EarningType,
                OvertimeMultiplier = sourcePmTran.OvertimeMultiplier,
                UseBillableQty = sourcePmTran.UseBillableQty,
                Allocated = false,
                Released = false,
                EndDate = entityPair.ReassignmentDate,
                StartDate = entityPair.ReassignmentDate                
            };

            var reassignmentDestTran = new PMTran
            {
                RefNbr = RegisterEntry.Document.Current.RefNbr,
                BranchID = sourcePmTran.BranchID,
                ProjectID = reassignmentDestination.ProjectID,
                TaskID = reassignmentDestination.TaskID,
                AccountGroupID = sourcePmTran.AccountGroupID,
                ResourceID = sourcePmTran.ResourceID,
                BAccountID = sourcePmTran.BAccountID,
                LocationID = sourcePmTran.LocationID,
                InventoryID = sourcePmTran.InventoryID,
                Description = sourcePmTran.Description,
                UOM = sourcePmTran.UOM,
                Qty = sourcePmTran.Qty,
                Billable = sourcePmTran.Billable,
                BillableQty = sourcePmTran.BillableQty,
                UnitRate = sourcePmTran.UnitRate,
                Amount = reassignmentValue.Amount,
                AccountID = sourcePmTran.AccountID,
                SubID = sourcePmTran.SubID,
                OffsetAccountID = sourcePmTran.OffsetAccountID ?? setupOffsetAccountID,
                OffsetSubID = sourcePmTran.OffsetSubID ?? setupOffsetAccountSubID,
                Date = entityPair.ReassignmentDate,
                FinPeriodID = entityPair.ReassignmentFinPeriodID,
                BatchNbr = null,
                EarningType = sourcePmTran.EarningType,
                OvertimeMultiplier = sourcePmTran.OvertimeMultiplier,
                UseBillableQty = sourcePmTran.UseBillableQty,
                Allocated = false,
                Released = false,
                EndDate = entityPair.ReassignmentDate,
                StartDate = entityPair.ReassignmentDate
            };


            var insertedSourceTran = RegisterEntry.Transactions.Insert(reassignmentSourceTran);
            var insertedDestTran = RegisterEntry.Transactions.Insert(reassignmentDestTran);

            RegisterEntry.Transactions.SetValueExt<PMTranExt.usrReassigned>(insertedSourceTran, true);
            RegisterEntry.Transactions.SetValueExt<PMTranExt.usrReassigned>(insertedDestTran, true);
            CachedValues.Add(new CachedValue(sourcePmTran, insertedSourceTran, insertedDestTran, reassignmentValue, entityPair.PMReassignmentID, entityPair.RevID));
        } 

        #endregion
    }
}
