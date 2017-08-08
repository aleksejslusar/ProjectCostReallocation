using System;
using System.Collections.Generic;
using System.Linq;
using ProjectCostReallocation.DAC;
using PX.Data;
using PX.Objects.PM;

namespace ProjectCostReallocation.BL.ReassignmentChain
{
    public class AddPMTransHandler: TransactionEntityBase
    {
        private readonly ProjectCostReassignmentEntry _reassignmentGraph;
        private readonly RegisterEntry _registerGraph;

        public AddPMTransHandler(PMCostReassignmentProcessor processor, PMCostReassigmentEntity entity) : base(processor, entity)
        {
            _reassignmentGraph = Processor.ReassignmentGraph;
            _registerGraph = Processor.RegisterGraph;
        }

        public override void ExecuteRequest()
        {            
            var reassignmentSource = GetReassignmentSource(entity.SourceLineID);
            var reassigmentDestination = GetReassignmentDestination(entity.DestinaitonLineID);
            var sourcePMTrans = GetSourseTransaction(reassignmentSource);
            
            //Validate if no any source transactions to reassign
            if (!sourcePMTrans.Any())
            {
                entity.ProcessingWarning = true;
                entity.ProcessingMessage = PMCostReassignmentMessages.REASSAIMENT_NOTHING_TO_REASSIGN_TRAN_NOT_FOUND;
                return;
            }

            //Validate if entityPair was reassigned earlier
            var canReassignmenPMTrans = new List<PMTran>();
            var currentReassignment = Processor.ReassignmentGraph.PMCostReassignment.Current;
            if (currentReassignment != null)
            {
                
                foreach (var sourcePMTran in sourcePMTrans)
                {
                    var processedDestTaskIds = Processor.ProcessedTransactions.Select(sourcePMTran.TranID, currentReassignment.PMReassignmentID, currentReassignment.RevID)
                                                                              .Select(i => i.GetItem<UsrPMCostReassignmentRunHistory>().DestinationTaskID)
                                                                              .ToList();

                    if (!processedDestTaskIds.Any() || !processedDestTaskIds.Contains(entity.DestinationTaskID))
                    {
                        canReassignmenPMTrans.Add(sourcePMTran);
                    }
                }
            }

            if (!canReassignmenPMTrans.Any())
            {
                entity.ProcessingWarning = true;
                entity.ProcessingMessage = PMCostReassignmentMessages.REASSAIMENT_NOTHING_TO_REASSIGN_TRAN_REQASSIGNED_EARLIER;
                return;
            }
           
            //Create transactions
            foreach (var sourcePMTran in canReassignmenPMTrans)
            {
                var balansedSourceAmount = CalculateBalanceSourceAmount(sourcePMTran);
                var reassignmentData = CalculateReassignmentAmount(sourcePMTran, balansedSourceAmount, reassigmentDestination);
                if (!reassignmentData.IsValid)
                {
                    entity.ProcessingWarning = true;
                    entity.ProcessingMessage = PMCostReassignmentMessages.REASSAIMENT_AMOUNT_ERROR;
                    return;
                }

                WriteTransactions(entity, reassignmentData, sourcePMTran, reassignmentSource, reassigmentDestination);
                if (Math.Abs(balansedSourceAmount) <= Math.Abs(reassignmentData.Amount))
                {
                    Processor.ReassignedSourcePMTrans.Add(sourcePMTran);
                }
            }
            entity.ProcessingSuccess = true;
            entity.ProcessingMessage = PMCostReassignmentMessages.REASSIGNMENT_SUCCESS;
            
            // Call next chain
            Successor?.ExecuteRequest();
        }

        #region Private
        private UsrPMCostReassignmentSource GetReassignmentSource(int? lineID)
        {
            return _reassignmentGraph.PMCostReassignmentSource.Search<UsrPMCostReassignmentSource.lineID>(lineID);
        }

        private UsrPMCostReassignmentDestination GetReassignmentDestination(int? lineID)
        {
            return _reassignmentGraph.PMCostReassignmentDestination.Search<UsrPMCostReassignmentDestination.lineID>(lineID);
        }

        private List<PMTran> GetSourseTransaction(UsrPMCostReassignmentSource reassignmentSource)
        {
            var pmTran = new PXSelect<PMTran, Where<PMTran.projectID, Equal<Required<PMTran.projectID>>,
                                                And<PMTran.taskID, Equal<Required<PMTran.taskID>>,
                                                And<PMTran.released, Equal<True>, 
                                                And<PMTranExt.usrReassigned, Equal<False>>>>>>(_registerGraph);

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
            var currentReassignment = Processor.ReassignmentGraph.PMCostReassignment.Current;
            if (currentReassignment != null)
            {
                var processedTransAmount = Math.Abs(Processor.ProcessedTransactions.Select(sourcePMTran.TranID, currentReassignment.PMReassignmentID, currentReassignment.RevID)
                                                                                   .Select(i => i.GetItem<PMTran>())
                                                                                   .Where(i => i.Amount > 0)
                                                                                   .Sum(i => Math.Round(i.Amount.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero)));

                var sourceTransAmount = Math.Round(sourcePMTran.Amount.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero);
                var existsTransAmount = Math.Round(processedTransAmount, 2, MidpointRounding.AwayFromZero);
                return sourcePMTran.Amount > 0 ? sourceTransAmount - existsTransAmount : sourceTransAmount + existsTransAmount;
            }

            return 0;
        }

        private ReassigmentValue CalculateReassignmentAmount(PMTran sourcePMTran, decimal balansedSourceAmount, UsrPMCostReassignmentDestination reassignmentDestination)
        {
            decimal sourceAmount = 0;
            decimal destAmount = 0;

            var totalReassignmentValue1 = _reassignmentGraph.PMCostReassignment.Current.ReassignmentValue1Total;
            var totalReassignmentValue2 = _reassignmentGraph.PMCostReassignment.Current.ReassignmentValue2Total;
            var reassigmentSelection = reassignmentDestination.ReassignmentSelection;

            var currentReassignment = Processor.ReassignmentGraph.PMCostReassignment.Current;
            if (currentReassignment != null)
            {
                var processedDestTaskIds = Processor.ProcessedTransactions.Select(sourcePMTran.TranID, currentReassignment.PMReassignmentID, currentReassignment.RevID)
                                                                                 .Select(i => i.GetItem<UsrPMCostReassignmentRunHistory>().DestinationTaskID)
                                                                                 .ToList();
                if (processedDestTaskIds.Any())
                {
                    foreach (var result in Processor.ReassignmentGraph.PMCostReassignmentDestination.Select())
                    {
                        var dest = result.GetItem<UsrPMCostReassignmentDestination>();
                        if (dest != null && processedDestTaskIds.Contains(dest.TaskID))
                        {
                            totalReassignmentValue1 -= dest.ReassignmentValue1;
                            totalReassignmentValue2 -= dest.ReassignmentValue2;
                        }
                    }
                }
            }
            
            switch (reassigmentSelection)
            {
                case "C": //Number of Units
                    {
                        sourceAmount = totalReassignmentValue2.GetValueOrDefault();
                        destAmount = reassignmentDestination.ReassignmentValue2.GetValueOrDefault();
                        if (destAmount == 0 || sourceAmount == 0)
                        {
                            throw new Exception(PMCostReassignmentMessages.REASSAIMENT_AMOUNT_ERROR_UNITS);
                        }
                    }
                    break;
                case "F": //Square Footage
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

            var percentage = Math.Round(destAmount / sourceAmount, 6, MidpointRounding.AwayFromZero);
            var amount = Math.Round(balansedSourceAmount * percentage, 3, MidpointRounding.AwayFromZero);

            var isValid = percentage > 0 && percentage <= 1 && amount != 0;
            return new ReassigmentValue(isValid, amount, sourceAmount, destAmount, percentage, reassigmentSelection);
        }

        private void WriteTransactions(PMCostReassigmentEntity entityPair, ReassigmentValue reassignmentValue, PMTran sourcePmTran, UsrPMCostReassignmentSource reassignmentSource, UsrPMCostReassignmentDestination reassignmentDestination)
        {
            var setupOffsetAccountID = Processor.Setup?.GetExtension<PMSetupExt>()?.UsrReassignmentAccountID;
            var setupOffsetAccountSubID = Processor.Setup?.GetExtension<PMSetupExt>()?.UsrReassignmentSubID;

            //Create Transactions
            var reassignmentSourceTran = new PMTran
            {
                RefNbr = _registerGraph.Document.Current.RefNbr,
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
                RefNbr = _registerGraph.Document.Current.RefNbr,
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


            var insertedSourceTran = _registerGraph.Transactions.Insert(reassignmentSourceTran);
            var insertedDestTran = _registerGraph.Transactions.Insert(reassignmentDestTran);

            _registerGraph.Transactions.SetValueExt<PMTranExt.usrReassigned>(insertedSourceTran, true);
            _registerGraph.Transactions.SetValueExt<PMTranExt.usrReassigned>(insertedDestTran, true);
            Processor.CachedValues.Add(new CachedValue(sourcePmTran, insertedSourceTran, insertedDestTran, reassignmentValue, entityPair.PMReassignmentID, entityPair.RevID));
        } 
        #endregion
    }
}
