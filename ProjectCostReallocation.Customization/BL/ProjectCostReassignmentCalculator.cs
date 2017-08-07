using System;
using System.Collections.Generic;
using System.Linq;
using ProjectCostReallocation.DAC;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.PM;

namespace ProjectCostReallocation.BL
{
    public class ProjectCostReassignmentCalculator: IDisposable
    {
        private readonly RegisterEntry _graph;

        private readonly PXSelectBase<UsrPMCostReassignmentSource> _reassignmentSource;
        private readonly PXSelectBase<UsrPMCostReassignmentDestination> _reassignmentDestination;
        private readonly PXSelectBase<UsrPMCostReassignmentDestination> _reassignmentDestinations;
        private readonly PXSelectBase<UsrPMCostReassignmentHistory> _usrPMCostReassignmentHistory;
        private readonly PXSelectBase<UsrPMCostReassignmentSourceTran> _usrPMCostReassignmentSourceTran;
        private readonly PXSelectBase<UsrPMCostReassignmentPercentage> _usrPMCostReassignmentPercentage;
        
        private readonly PXSelectBase<PMSetup> _pmSetup;

        private readonly List<PMTran> _reassignedPMTrans = new List<PMTran>();   
        private readonly List<CachedValue> _cachedValues = new List<CachedValue>();
        private PMSetup _pmSetupRow;

        public ProjectCostReassignmentCalculator(RegisterEntry graph)
        {            
            _graph = graph;

            //Init selects
            _reassignmentDestinations = new PXSelect<UsrPMCostReassignmentDestination, Where<UsrPMCostReassignmentDestination.pMReassignmentID, Equal<Required<UsrPMCostReassignmentDestination.pMReassignmentID>>>>(_graph);
            _reassignmentSource = new PXSelect<UsrPMCostReassignmentSource, Where<UsrPMCostReassignmentSource.lineID, Equal<Required<UsrPMCostReassignmentSource.lineID>>>>(_graph);
            _reassignmentDestination = new PXSelect<UsrPMCostReassignmentDestination, Where<UsrPMCostReassignmentDestination.lineID, Equal<Required<UsrPMCostReassignmentDestination.lineID>>>>(_graph);        
            _usrPMCostReassignmentHistory = new PXSelect<UsrPMCostReassignmentHistory>(_graph);
            _usrPMCostReassignmentSourceTran = new PXSelect<UsrPMCostReassignmentSourceTran>(_graph);
            _usrPMCostReassignmentPercentage = new PXSelect<UsrPMCostReassignmentPercentage>(_graph);
            _pmSetup = new PXSelect<PMSetup>(_graph);
        }

        #region Public

        public void ProcessReassigment(IGrouping<string, PMCostReassigmentEntity> reassigment)
        {
            _pmSetupRow = _pmSetup.Select().FirstOrDefault();
            using (var ts = new PXTransactionScope())
            {
                // === Transaction scope ===
                //1. Create PMRegister
                //2. Process each reassignmentEntity row and create transactions
                //3. Checking whether to save or not             
                //4. Save changes
                //5. Write data information to history and log tables
                //6. Mark source transations as reassigned
                //7. Release PMRegister

                //1
                WriteRegisterRow(reassigment.Key);

                //2
                foreach (var entity in reassigment)
                {
                    ProcessReassignmentEntity(entity);
                }

                //3
                if (_graph.Document.Current == null || !_graph.Transactions.Select().Any())
                {
                    return;
                }

                //4                             
                _graph.Actions.PressSave();

                //5
                foreach (var cachedValue in _cachedValues)
                {                    
                    WriteUsrPMCostReassignmentHistory(cachedValue.InsertedSourceTran.TranID, cachedValue.PMReassignmentID);
                    WriteUsrPMCostReassignmentHistory(cachedValue.InsertedDestTran.TranID, cachedValue.PMReassignmentID);

                    WriteUsrPMCostReassignmentSourceTran(cachedValue.PMReassignmentID, cachedValue.SourcePMTran.TranID, cachedValue.InsertedSourceTran.TranID);
                    WriteUsrPMCostReassignmentSourceTran(cachedValue.PMReassignmentID, cachedValue.SourcePMTran.TranID, cachedValue.InsertedDestTran.TranID);

                    WriteUsrPMCostReassignmentPercentage(cachedValue.PMReassignmentID, cachedValue.InsertedDestTran.TranID, cachedValue.ReassigmentValue.Selection, cachedValue.ReassigmentValue.Percentage, cachedValue.ReassigmentValue.SourceAmount, cachedValue.ReassigmentValue.DestAmount);
                }

                _usrPMCostReassignmentHistory.Cache.Persist(PXDBOperation.Insert);
                _usrPMCostReassignmentSourceTran.Cache.Persist(PXDBOperation.Insert);
                _usrPMCostReassignmentPercentage.Cache.Persist(PXDBOperation.Insert);
                
                //6
                UpdatePMTranUsrReassigned(_reassignedPMTrans);

                //7
                ReleasePMRegister();

                ts.Complete();
            }
        }

        public void Dispose()
        {
            _reassignmentSource.Cache.Clear();
            _reassignmentDestination.Cache.Clear();            
            _usrPMCostReassignmentHistory.Cache.Clear();
            _usrPMCostReassignmentSourceTran.Cache.Clear();
            _usrPMCostReassignmentPercentage.Cache.Clear();
        }

        #endregion

        #region Private

        private void ProcessReassignmentEntity(PMCostReassigmentEntity entity)
        {
            var reassignmentSource = GetReassignmentSource(entity.SourceLineID);
            var reassigmentDestination = GetReassignmentDestination(entity.DestinaitonLineID);
            var sourcePMTrans = GetSourseTransaction(reassignmentSource);

            if (!sourcePMTrans.Any())
            {
                entity.ProcessingWarning = true;
                entity.ProcessingMessage = ProjectCostReassignmentMessages.REASSAIMENT_NOTHING_TO_REASSIGN_TRAN_NOT_FOUND;
                return;
            }

            if (sourcePMTrans.All(e => e.GetExtension<PMTranExt>().UsrReassigned == true))
            {
                entity.ProcessingWarning = true;
                entity.ProcessingMessage =
                    ProjectCostReassignmentMessages.REASSAIMENT_NOTHING_TO_REASSIGN_TRAN_REQASSIGNED_EARLIER;
                return;
            }


            //Create transactions
            foreach (var sourcePMTran in sourcePMTrans.Where(e => e.GetExtension<PMTranExt>().UsrReassigned != true))
            {
                var reassignmentValues = CalculateReassignmentAmount(sourcePMTran, reassignmentSource, reassigmentDestination);
                if (!reassignmentValues.IsValid)
                {
                    entity.ProcessingWarning = true;
                    entity.ProcessingMessage = ProjectCostReassignmentMessages.REASSAIMENT_AMOUNT_ERROR;
                    return;
                }

                WriteTransactions(entity, reassignmentValues, sourcePMTran, reassignmentSource, reassigmentDestination);
                if (_reassignedPMTrans.All(e => e.TranID != sourcePMTran.TranID))
                {
                    _reassignedPMTrans.Add(sourcePMTran);
                }
            }
            entity.ProcessingSuccess = true;
            entity.ProcessingMessage = ProjectCostReassignmentMessages.REASSIGNMENT_SUCCESS;

        }

        private UsrPMCostReassignmentSource GetReassignmentSource(int? lineID)
        {
            return _reassignmentSource.Select(lineID);
        }

        private UsrPMCostReassignmentDestination GetReassignmentDestination(int? lineID)
        {
            return _reassignmentDestination.Select(lineID);
        }

        private List<PMTran> GetSourseTransaction(UsrPMCostReassignmentSource reassignmentSource)
        {
            var pmTranSource = new PXSelect<PMTran, Where<PMTran.projectID, Equal<Required<PMTran.projectID>>,
                                        And<PMTran.taskID, Equal<Required<PMTran.taskID>>>>>(_graph);
            if (reassignmentSource.AccountGroupFrom != null && reassignmentSource.AccountGroupTo != null)
            {
                pmTranSource.WhereAnd<Where<PMTran.accountGroupID, Between<Required<PMTran.accountGroupID>, Required<PMTran.accountGroupID>>>>();
            }

            if ((reassignmentSource.AccountGroupFrom != null && reassignmentSource.AccountGroupTo == null) || 
                (reassignmentSource.AccountGroupFrom == null && reassignmentSource.AccountGroupTo != null))
            {
                pmTranSource.WhereAnd<Where<PMTran.accountGroupID, Equal<Required<PMTran.accountGroupID>>>>();
            }

            return pmTranSource.Select(reassignmentSource.ProjectID, reassignmentSource.TaskID, reassignmentSource.AccountGroupFrom, reassignmentSource.AccountGroupTo)
                               .Select(e => (PMTran)e)
                               .ToList();
        }

        private ReassigmentValue CalculateReassignmentAmount(PMTran sourceTran, IUsrPMCostReassignmentProjectAndTask reassignmentSource, UsrPMCostReassignmentDestination reassignmentDestination)
        {
            decimal sourceAmount = 0;
            decimal destAmount = 0;

            var reassignmentDestinations = _reassignmentDestinations.Select(reassignmentSource.PMReassignmentID).Where(e => e.GetItem<PMTask>().Status != ProjectTaskStatus.Completed).ToList();
            var totalReassignmentValue1 = reassignmentDestinations.Sum(d => ((UsrPMCostReassignmentDestination)d).ReassignmentValue1);
            var totalReassignmentValue2 = reassignmentDestinations.Sum(d => ((UsrPMCostReassignmentDestination)d).ReassignmentValue2);
            var reassigmentSelection = reassignmentDestination.ReassignmentSelection;

            switch (reassigmentSelection)
            {
                case "C": //Number of Units
                {
                    sourceAmount = totalReassignmentValue2.GetValueOrDefault();
                    destAmount = reassignmentDestination.ReassignmentValue2.GetValueOrDefault();
                    if (destAmount == 0 || sourceAmount == 0)
                    {
                        throw new Exception(ProjectCostReassignmentMessages.REASSAIMENT_AMOUNT_ERROR_UNITS);
                    }
                }
                    break;
                case "F": //Square Footage
                {
                    sourceAmount = totalReassignmentValue1.GetValueOrDefault();
                    destAmount = reassignmentDestination.ReassignmentValue1.GetValueOrDefault();
                    if (destAmount == 0 || sourceAmount == 0)
                    {
                        throw new Exception(ProjectCostReassignmentMessages.REASSAIMENT_AMOUNT_ERROR_SF);
                    }
                }
                    break;
            }

            var percentage = destAmount/sourceAmount;
            var amount = sourceTran.Amount.GetValueOrDefault()*percentage;

            var isValid = percentage > 0 && percentage < 1;
            return new ReassigmentValue(isValid, amount, sourceAmount, destAmount, percentage, reassigmentSelection);
        }

        private void WriteRegisterRow(string pmReassignmentID)
        {
            var pmRegisterRow = new PMRegister
            {
                Module = BatchModule.PM,
                Status = PMRegister.status.Balanced,
                Description = "Costs Reassignment " + pmReassignmentID
            };
            var newRegisterRow = _graph.Document.Insert(pmRegisterRow);
            _graph.Document.Cache.SetValueExt<PMRegisterExt.usrIsReassignment>(newRegisterRow, true);
        }

        private void WriteTransactions(PMCostReassigmentEntity entity, ReassigmentValue reassignmentValue, PMTran sourcePmTran, UsrPMCostReassignmentSource reassignmentSource, UsrPMCostReassignmentDestination reassignmentDestination)
        {
            var setupOffsetAccountID = _pmSetupRow?.GetExtension<PMSetupExt>()?.UsrReassignmentAccountID;
            var setupOffsetAccountSubID = _pmSetupRow?.GetExtension<PMSetupExt>()?.UsrReassignmentSubID;

            //Create Transactions
            var reassignmentSourceTran = new PMTran
            {
                RefNbr = _graph.Document.Current.RefNbr,
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
                Amount = (reassignmentValue.Amount*-1),
                AccountID = sourcePmTran.AccountID,
                SubID = sourcePmTran.SubID,
                OffsetAccountID = sourcePmTran.OffsetAccountID ?? setupOffsetAccountID,
                OffsetSubID = sourcePmTran.OffsetSubID ?? setupOffsetAccountSubID,
                Date = entity.ReassignmentDate,
                FinPeriodID = entity.ReassignmentFinPeriodID,
                BatchNbr = null,
                EarningType = sourcePmTran.EarningType,
                OvertimeMultiplier = sourcePmTran.OvertimeMultiplier,
                UseBillableQty = sourcePmTran.UseBillableQty,
                Allocated = false,
                Released = false,
                EndDate = entity.ReassignmentDate,
                StartDate = entity.ReassignmentDate
            };

            var reassignmentDestTran = new PMTran
            {
                RefNbr = _graph.Document.Current.RefNbr,
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
                Date = entity.ReassignmentDate,
                FinPeriodID = entity.ReassignmentFinPeriodID,
                BatchNbr = null,
                EarningType = sourcePmTran.EarningType,
                OvertimeMultiplier = sourcePmTran.OvertimeMultiplier,
                UseBillableQty = sourcePmTran.UseBillableQty,
                Allocated = false,
                Released = false,
                EndDate = entity.ReassignmentDate,
                StartDate = entity.ReassignmentDate
            };


            var insertedSourceTran = _graph.Transactions.Insert(reassignmentSourceTran);
            var insertedDestTran = _graph.Transactions.Insert(reassignmentDestTran);

            _graph.Transactions.SetValueExt<PMTranExt.usrReassigned>(insertedSourceTran, true);
            _graph.Transactions.SetValueExt<PMTranExt.usrReassigned>(insertedDestTran, true);
            _cachedValues.Add(new CachedValue(sourcePmTran, insertedSourceTran, insertedDestTran, reassignmentValue,
                entity.PMReassignmentID));
        }

        private void WriteUsrPMCostReassignmentHistory(long? pmTranID, string pmReassignmentID)
        {
            var historyRow = new UsrPMCostReassignmentHistory
            {
                PMReassignmentID = pmReassignmentID,
                TranID = pmTranID
            };

            _usrPMCostReassignmentHistory.Insert(historyRow);
        }

        private void WriteUsrPMCostReassignmentSourceTran(string reassignmentId, long? sourceTranID, long? destTranID)
        {
            var row = new UsrPMCostReassignmentSourceTran
            {
                PMReassignmentID = reassignmentId,
                SourceTranID = sourceTranID,
                TranID = destTranID
            };

            _usrPMCostReassignmentSourceTran.Insert(row);
        }

        private void WriteUsrPMCostReassignmentPercentage(string reassignmentId, long? tranID, string reassignmentSelection, decimal? percentage, decimal? sourceAmount, decimal? destAmount)
        {
            var row = new UsrPMCostReassignmentPercentage
            {
                PMReassignmentID = reassignmentId,
                TranID = tranID,
                ReassignmentSelection = reassignmentSelection,
                Percentage = percentage,
                SourceValue = sourceAmount,
                DestinationValue = destAmount
            };

            _usrPMCostReassignmentPercentage.Insert(row);
        }

        private void UpdatePMTranUsrReassigned(IEnumerable<PMTran> sourceTrans)
        {
            foreach (var sourceTran in sourceTrans)
            {
                sourceTran.GetExtension<PMTranExt>().UsrReassigned = true;
                _graph.Transactions.Update(sourceTran);
                _graph.Persist(typeof (PMTran), PXDBOperation.Update);
            }
        }

        private void ReleasePMRegister()
        {
            PMSetup setup = PXSelect<PMSetup>.Select(_graph);
            var value = setup.GetExtension<PMSetupExt>()?.UsrAutoReleaseReassignment;
            if (value.GetValueOrDefault())
            {

                if (_graph.Document.Current != null)
                {
                    _graph.Actions.PressSave();
                    _graph.ReleaseDocument(_graph.Document.Current);
                }
            }
        }

        #endregion
        
    }
}
