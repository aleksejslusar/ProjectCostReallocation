using System;
using System.Collections.Generic;
using System.Linq;
using PX.Data;
using PX.Objects.PM;

namespace ProjectCostReallocation
{
    public class AddPMTransHandler: TransactionEntityBase
    {
        public AddPMTransHandler(ProjectCostReassignmentEntry reassignmentEntry, RegisterEntry registerEntry, PMCostReassigmentEntity entity) : base(reassignmentEntry, registerEntry, entity)
        {

        }

        protected override void HandlerRequest()
        {            
            var reassignmentSource = ReassignmentEntry.PMCostReassignmentSource.Search<UsrPMCostReassignmentSource.lineID>(Entity.SourceLineID);
            var reassigmentDestination = ReassignmentEntry.PMCostReassignmentDestination.Search<UsrPMCostReassignmentDestination.lineID>(Entity.DestinaitonLineID);
            var sourcePMTrans = GetSourseTransaction(reassignmentSource);
            
            //Validate if no any source transactions to reassign
            if (!sourcePMTrans.Any())
            {
                Entity.ProcessingWarning = true;
                Entity.ProcessingMessage = PMCostReassignmentMessages.REASSAIMENT_NOTHING_TO_REASSIGN_TRAN_NOT_FOUND;
                return;
            }

            //Validate if entityPair was reassigned earlier
            var canReassignmenPMTrans = new List<PMTran>();
            var currentReassignment = ReassignmentEntry.PMCostReassignment.Current;
            if (currentReassignment != null)
            {
                
                foreach (var sourcePMTran in sourcePMTrans)
                {
                    var processedDestTaskIds = ReassignmentEntry.ProcessedTransactions.Select(sourcePMTran.TranID, currentReassignment.PMReassignmentID, currentReassignment.RevID)
                                                                              .Select(i => i.GetItem<UsrPMCostReassignmentRunHistory>().DestinationTaskID)
                                                                              .ToList();

                    if (!processedDestTaskIds.Any() || !processedDestTaskIds.Contains(Entity.DestinationTaskID))
                    {
                        canReassignmenPMTrans.Add(sourcePMTran);
                    }
                }
            }

            if (!canReassignmenPMTrans.Any())
            {
                Entity.ProcessingWarning = true;
                Entity.ProcessingMessage = PMCostReassignmentMessages.REASSAIMENT_NOTHING_TO_REASSIGN_TRAN_REQASSIGNED_EARLIER;
                return;
            }
           
            //Create transactions
            foreach (var sourcePMTran in canReassignmenPMTrans)
            {
                var balansedSourceAmount = CalculateBalanceSourceAmount(sourcePMTran);
                var reassignmentData = CalculateReassignmentAmount(sourcePMTran, balansedSourceAmount, reassigmentDestination);
                if (!reassignmentData.IsValid)
                {
                    Entity.ProcessingWarning = true;
                    Entity.ProcessingMessage = PMCostReassignmentMessages.REASSAIMENT_AMOUNT_ERROR;
                    return;
                }

                WriteTransactions(Entity, reassignmentData, sourcePMTran, reassignmentSource, reassigmentDestination);
                if (Math.Abs(balansedSourceAmount) <= Math.Abs(reassignmentData.Amount))
                {
                    ReassignedSourcePMTrans.Add(sourcePMTran);
                }
            }
            Entity.ProcessingSuccess = true;
            Entity.ProcessingMessage = PMCostReassignmentMessages.REASSIGNMENT_SUCCESS;            
        }

        #region Private

        private List<PMTran> GetSourseTransaction(UsrPMCostReassignmentSource reassignmentSource)
        {
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

            return pmTran.Select(reassignmentSource.ProjectID, reassignmentSource.TaskID, reassignmentSource.AccountGroupFrom, reassignmentSource.AccountGroupTo)
                         .Select(e => (PMTran)e)
                         .ToList();
        }

        private decimal CalculateBalanceSourceAmount(PMTran sourcePMTran)
        {
            var currentReassignment = ReassignmentEntry.PMCostReassignment.Current;
            if (currentReassignment != null)
            {
                var existsTransAmount = Math.Abs(ReassignmentEntry.ProcessedTransactions.Select(sourcePMTran.TranID, currentReassignment.PMReassignmentID, currentReassignment.RevID)
                                                                                   .Select(i => i.GetItem<PMTran>())
                                                                                   .Where(i => i.Amount > 0)
                                                                                   .Sum(i => i.Amount.GetValueOrDefault()));

                var sourceTransAmount = sourcePMTran.Amount.GetValueOrDefault();               
                var result = sourcePMTran.Amount > 0 ? sourceTransAmount - existsTransAmount : sourceTransAmount + existsTransAmount;
                return Math.Round(result, 2, MidpointRounding.AwayFromZero);
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

            var percentage = destAmount / sourceAmount;
            var amount = balansedSourceAmount * percentage;

            var isValid = percentage > 0 && percentage <= 1 && amount != 0;
            return new ReassigmentValue(isValid, amount, sourceAmount, destAmount, percentage, reassigmentSelection);
        }

        private void WriteTransactions(PMCostReassigmentEntity entityPair, ReassigmentValue reassignmentValue, PMTran sourcePmTran, UsrPMCostReassignmentSource reassignmentSource, UsrPMCostReassignmentDestination reassignmentDestination)
        {
            PMSetup setup = ReassignmentEntry.PMSetupSelect.Select().FirstOrDefault();

            var setupOffsetAccountID = setup?.GetExtension<PMSetupExt>()?.UsrReassignmentAccountID;
            var setupOffsetAccountSubID = setup?.GetExtension<PMSetupExt>()?.UsrReassignmentSubID;

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
