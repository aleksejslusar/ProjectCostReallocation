using System.Collections.Generic;
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.PM;
using Messages = PX.Objects.PM.Messages;

namespace ProjectCostReallocation
{

    public class RegisterEntryExt : PXGraphExtension<RegisterEntry>
    {
        public PXSelect<UsrPMCostReassignmentRunHistory, Where<UsrPMCostReassignmentRunHistory.pMReassignmentID, Equal<Required<UsrPMCostReassignmentRunHistory.pMReassignmentID>>,
                                                                And<UsrPMCostReassignmentRunHistory.revID, Equal<Required<UsrPMCostReassignmentRunHistory.revID>>,
                                                                And2<Where<UsrPMCostReassignmentRunHistory.sourceTranID, Equal<Required<UsrPMCostReassignmentRunHistory.sourceTranID>>>,
                                                                  Or<UsrPMCostReassignmentRunHistory.destinationTranID, Equal<Required<UsrPMCostReassignmentRunHistory.destinationTranID>>>>>>> UsrPMCostReassignmentRunHistorySelect;
        public PXSelect<UsrPMCostReassignment, Where<UsrPMCostReassignment.pMReassignmentID, Equal<Required<UsrPMCostReassignment.pMReassignmentID>>>> UsrPMCostReassignmentSelect;
        public PXSelect<UsrPMCostReassignmentSourceTran, Where<UsrPMCostReassignmentSourceTran.tranID, Equal<Required<UsrPMCostReassignmentSourceTran.tranID>>>> UsrPMCostReassignmentSourceTranSelect;

        public PXSelect<UsrPMCostReassignmentHistory, Where<UsrPMCostReassignmentHistory.tranID, Equal<Required<UsrPMCostReassignmentHistory.tranID>>>> UsrPMCostReassignmentHistorySelect;
        public PXSelect<UsrPMCostReassignmentPercentage, Where<UsrPMCostReassignmentPercentage.tranID, Equal<Required<UsrPMCostReassignmentPercentage.tranID>>>> UsrPMCostReassignmentPercentageSelect;
        public PXSelect<PMTran, Where<PMTran.tranID, Equal<Required<PMTran.tranID>>>> SourceTranSelect; 

        #region Actions

        public PXAction<PMRegister> reverseReassignment;

        [PXProcessButton(Tooltip = "Reverse Reassignment")]
        [PXUIField(DisplayName = "Reverse Reassignment")]
        protected void ReverseReassignment()
        {
            PXLongOperation.StartOperation(Base, () =>
            {
                //Check for reversed earlier
                PMRegister reversalExist = PXSelect<PMRegister,
                                             Where<PMRegister.module, Equal<Current<PMRegister.module>>,
                                                And<PMRegister.origRefNbr, Equal<Current<PMRegister.refNbr>>,
                                                And<PMRegister.origDocNbr, Equal<Current<PMRegister.refNbr>>>>>>.Select(Base);
                if (reversalExist != null)
                {
                    throw new PXException(string.Format(PMCostReassignmentMessages.REASSIGNMENT_REVERSE_ERROR, reversalExist.RefNbr));
                }
                //Revert
                var revertedEntry = PerformReverseReassignment();

                //Release
                revertedEntry.Document.Current = PXSelect<PMRegister, 
                                                    Where<PMRegister.module, Equal<Current<PMRegister.module>>, 
                                                      And<PMRegister.origRefNbr, Equal<Current<PMRegister.refNbr>>>>>.Select(Base);
                               
                if (revertedEntry.Document.Current.Released != true)
                {
                    PXLongOperation.StartOperation(revertedEntry.UID, () =>
                    {                        
                        revertedEntry.ReleaseDocument(revertedEntry.Document.Current);                                              
                    });

                    PXLongOperation.WaitCompletion(revertedEntry.UID);
                }

                //Redirect
                throw new PXRedirectRequiredException(revertedEntry, "Open Reversal");
            });
        }

        #endregion

        #region Event Handlers

        protected virtual void PMRegister_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            var row = e.Row as PMRegister;
            if (row != null)
            {
                var usrIsReassignment = PXCache<PMRegister>.GetExtension<PMRegisterExt>(row).UsrIsReassignment.GetValueOrDefault();
                var isCanBeReverted = (usrIsReassignment && (row.OrigDocType != PMOrigDocType.Reversal) && row.Released == true);
                reverseReassignment.SetEnabled(isCanBeReverted);
                Base.Document.Cache.AllowUpdate = row.Released != true && row.Module == BatchModule.PM;
                Base.Document.Cache.AllowDelete = row.Released != true && row.Module == BatchModule.PM;

                var isCanEditTransactions = !usrIsReassignment;
                Base.Transactions.Cache.AllowDelete = isCanEditTransactions;
                Base.Transactions.Cache.AllowUpdate = isCanEditTransactions;
                Base.Transactions.Cache.AllowInsert = isCanEditTransactions;                
            }
        }

        protected virtual void PMRegister_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
        {
            var tranRows = Base.Caches<PMTran>().Deleted.ToArray<PMTran>();
            if (!tranRows.Any()) return;
            DeleteRelatedRecords(tranRows);
        }

        #endregion

        #region Private
        private RegisterEntry PerformReverseReassignment()
        {
            var register = Base.Document.Current;
            var transactions = Base.Transactions.Select();
            var newTrans = new Dictionary<PMTran, PMTran>();
            if (register != null)
            {
                RegisterEntry target;
                using (new PXConnectionScope())
                {
                    using (var ts = new PXTransactionScope())
                    {
                        target = PXGraph.CreateInstance<RegisterEntry>();
                        //Get target extention context
                        var targetExt = target.GetExtension<RegisterEntryExt>();

                        //Create PMRegister row
                        var doc = target.Document.Insert();
                        doc.Module = BatchModule.PM;
                        doc.Description = register.Description + " " + Messages.Reversal;
                        doc.OrigDocType = PMOrigDocType.Reversal;
                        doc.OrigDocNbr = register.RefNbr;
                        doc.OrigRefNbr = register.RefNbr;
                        PXCache<PMRegister>.GetExtension<PMRegisterExt>(doc).UsrIsReassignment = true;                                               

                        //Create transaction row
                        foreach (PMTran pmTran in transactions)
                        {                                                 
                            var tran = target.Transactions.Insert();
                            tran.RefNbr = doc.RefNbr;
                            tran.BranchID = pmTran.BranchID;
                            tran.ProjectID = pmTran.ProjectID;
                            tran.TaskID = pmTran.TaskID;
                            tran.AccountGroupID = pmTran.AccountGroupID;
                            tran.ResourceID = pmTran.ResourceID;
                            tran.BAccountID = pmTran.BAccountID;
                            tran.LocationID = pmTran.LocationID;
                            tran.InventoryID = pmTran.InventoryID;
                            tran.Description = pmTran.Description;
                            tran.UOM = pmTran.UOM;
                            tran.Qty = pmTran.Qty * -1;
                            tran.Billable = pmTran.Billable;
                            tran.BillableQty = pmTran.BillableQty * -1;
                            tran.UnitRate = 0;
                            tran.Amount = pmTran.Amount * -1;
                            tran.AccountID = pmTran.AccountID;
                            tran.SubID = pmTran.SubID;
                            tran.OffsetAccountID = pmTran.OffsetAccountID;
                            tran.OffsetSubID = pmTran.OffsetSubID;
                            tran.Date = target.Accessinfo.BusinessDate;
                            tran.FinPeriodID = FinPeriodIDAttribute.FindFinPeriodByDate(target, target.Accessinfo.BusinessDate).FinPeriodID;
                            tran.BatchNbr = null;
                            tran.EarningType = pmTran.EarningType;
                            tran.OvertimeMultiplier = pmTran.OvertimeMultiplier;
                            tran.UseBillableQty = pmTran.UseBillableQty;
                            tran.Allocated = false;
                            tran.Released = false;
                            tran.EndDate = tran.Date;
                            tran.StartDate = tran.Date;
                            tran.OrigTranID = pmTran.TranID;
                            target.Transactions.SetValueExt<PMTranExt.usrReassigned>(tran, true);
                            
                            //Delete UsrPMCostReassignmentRunHistory
                            targetExt.DeleteRelatedReassignmentRunHistory(pmTran);

                            //Save new tran to list
                            newTrans.Add(pmTran, tran);
                        }

                        //Write history
                        target.Actions.PressSave();
                        foreach (var tran in newTrans)
                        {
                            targetExt.WriteUsrPMCostReassignmentHistory(tran.Key, tran.Value);
                        }

                        //Reset tokens
                        if (newTrans.Any())
                        {
                            targetExt.ResetSourceTranReassignmentToken(newTrans.First().Key.TranID);
                        }
                        

                        ts.Complete();
                    }
                }

                target.Actions.PressSave();
                return target;
            }

            throw new PXException("Error during reverse reassignment operation");
        }

        private void DeleteRelatedRecords(IEnumerable<PMTran> tranRows)
        {
            using (new PXConnectionScope())
            {
                using (var ts = new PXTransactionScope())
                {
                    foreach (var tran in tranRows)
                    {
                        UsrPMCostReassignmentSourceTran reassignmentSourceTran = UsrPMCostReassignmentSourceTranSelect.Select(tran.TranID).FirstOrDefault();
                        if (reassignmentSourceTran == null) continue;

                        //Make source transaction available for reassignment again
                        var sourceTran = SourceTranSelect.SelectSingle(reassignmentSourceTran.SourceTranID);
                        
                        if (sourceTran != null)
                        {
                            if (PXCache<PMTran>.GetExtension<PMTranExt>(sourceTran).UsrReassigned.GetValueOrDefault())
                            {
                                PXCache<PMTran>.GetExtension<PMTranExt>(sourceTran).UsrReassigned = false;
                                SourceTranSelect.Update(sourceTran);
                            }
                        }

                        //Delete UsrPMCostReassignmentPercentage
                        foreach (var reassignmentPercentage in UsrPMCostReassignmentPercentageSelect.Select(reassignmentSourceTran.TranID))
                        {
                            UsrPMCostReassignmentPercentageSelect.Delete(reassignmentPercentage);
                        }

                        //Delete UsrPMCostReassignmentHistory
                        foreach (var usrReassignmentHistory in UsrPMCostReassignmentHistorySelect.Select(reassignmentSourceTran.TranID))
                        {
                            UsrPMCostReassignmentHistorySelect.Delete(usrReassignmentHistory);
                        }

                        //Delete UsrPMCostReassignmentRunHistory
                        DeleteRelatedReassignmentRunHistory(tran);

                        //Delete UsrPMCostReassignmentSourceTran
                        UsrPMCostReassignmentSourceTranSelect.Delete(reassignmentSourceTran);
                    }

                    ts.Complete();
                }
            }

            //Save changes to DB
            Base.Actions.PressSave();
        }

        private UsrPMCostReassignment GetReassignmentByTranID(PMTran tran)
        {
            UsrPMCostReassignmentSourceTran reassignmentSourceTran = UsrPMCostReassignmentSourceTranSelect.Select(tran.TranID).FirstOrDefault();
            if (reassignmentSourceTran != null)
            {
                UsrPMCostReassignment reasignment = UsrPMCostReassignmentSelect.Select(reassignmentSourceTran.PMReassignmentID);
                return reasignment;
            }
            return null;
        }

        private void DeleteRelatedReassignmentRunHistory(PMTran tran)
        {
            //Get reassignmemt
            var reassignmemt = GetReassignmentByTranID(tran);
            if (reassignmemt != null)
            {
                //Delete UsrPMCostReassignmentRunHistory
                var reassignmentRunHistory = UsrPMCostReassignmentRunHistorySelect.Select(reassignmemt.PMReassignmentID, reassignmemt.RevID, tran.TranID);
                if (reassignmentRunHistory != null)
                {
                    UsrPMCostReassignmentRunHistorySelect.Delete(reassignmentRunHistory);                    
                }
            }
        }

        private void WriteUsrPMCostReassignmentHistory(PMTran currentTran, PMTran newTran)
        {            
            //Get reassignmemt
            var reassignmemt = GetReassignmentByTranID(currentTran);
            if (reassignmemt != null)
            {
                var historyRow = UsrPMCostReassignmentHistorySelect.Insert();
                if (historyRow != null)
                {
                    historyRow.PMReassignmentID = reassignmemt.PMReassignmentID;
                    historyRow.TranID = newTran.TranID;                    
                }
            }
        }

        private void ResetSourceTranReassignmentToken(long? tranId)
        {
            UsrPMCostReassignmentSourceTran reassignmentSourceTran = UsrPMCostReassignmentSourceTranSelect.Select(tranId).FirstOrDefault();
            if (reassignmentSourceTran == null) return;

            //Make source transaction available for reassignment again
            var sourceTran = SourceTranSelect.SelectSingle(reassignmentSourceTran.SourceTranID);

            if (sourceTran != null)
            {
                if (PXCache<PMTran>.GetExtension<PMTranExt>(sourceTran).UsrReassigned.GetValueOrDefault())
                {
                    PXCache<PMTran>.GetExtension<PMTranExt>(sourceTran).UsrReassigned = false;
                    SourceTranSelect.Update(sourceTran);
                }
            }
        }

        #endregion

    }
}