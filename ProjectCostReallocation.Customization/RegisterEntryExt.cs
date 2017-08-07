using System.Collections.Generic;
using System.Linq;
using ProjectCostReallocation.BL;
using ProjectCostReallocation.DAC;
using PX.Common;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.PM;
using Messages = PX.Objects.PM.Messages;

namespace ProjectCostReallocation
{

    public class RegisterEntryExt : PXGraphExtension<RegisterEntry>
    {

        #region Actions

        public PXAction<PMRegister> reverseReassignment;

        [PXProcessButton(Tooltip = "Reverse Reassignment")]
        [PXUIField(DisplayName = "Reverse Reassignment")]
        protected void ReverseReassignment()
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
            PerformReverseReassignment();

        }

        #endregion

        #region Event Handlers

        protected virtual void PMRegister_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            var row = e.Row as PMRegister;
            if (row != null)
            {
                var isCanBeReverted = (row.GetExtension<PMRegisterExt>().UsrIsReassignment.GetValueOrDefault() && (row.OrigDocType != PMOrigDocType.Reversal) && row.Released == true);
                reverseReassignment.SetEnabled(isCanBeReverted);
                Base.Document.Cache.AllowUpdate = row.Released != true && row.Module == BatchModule.PM;
                Base.Document.Cache.AllowDelete = row.Released != true && row.Module == BatchModule.PM;
                //Add caches
                Base.Views.Caches.Add(typeof(UsrPMCostReassignmentSourceTran));
                Base.Views.Caches.Add(typeof(UsrPMCostReassignmentHistory));
                Base.Views.Caches.Add(typeof(UsrPMCostReassignmentPercentage));
                Base.Views.Caches.Add(typeof(UsrPMCostReassignmentRunHistory));

                var isCanEditTransactions = !row.GetExtension<PMRegisterExt>().UsrIsReassignment.GetValueOrDefault();
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

        protected virtual void PMRegister_RowPersisted(PXCache cache, PXRowPersistedEventArgs e)
        {
            if (e.Operation == PXDBOperation.Delete)
            {
                Base.Caches<UsrPMCostReassignmentSourceTran>().Clear();
                Base.Caches<UsrPMCostReassignmentPercentage>().Clear();
            }
        }

        #endregion

        #region Private
        private void PerformReverseReassignment()
        {
            var batchView = new PXSelect<Batch, Where<Batch.batchNbr, Equal<Required<Batch.batchNbr>>>>(Base);
            var current = Base.Document.Current;
            if (current != null)
            {
                RegisterEntry target;
                using (new PXConnectionScope())
                {
                    using (var ts = new PXTransactionScope())
                    {
                        target = PXGraph.CreateInstance<RegisterEntry>();
                        //Create PMRegister row
                        var doc = (PMRegister)target.Document.Cache.Insert();
                        doc.Module = BatchModule.PM;
                        doc.Description = current.Description + " " + Messages.Reversal;
                        doc.OrigDocType = PMOrigDocType.Reversal;
                        doc.OrigDocNbr = current.RefNbr;
                        doc.OrigRefNbr = current.RefNbr;
                        doc.GetExtension<PMRegisterExt>().UsrIsReassignment = true;

                        //Create transaction row
                        foreach (PMTran pmTran in Base.Transactions.Select())
                        {
                            var batch = batchView.Select(pmTran.BatchNbr);                            
                            var tran = (PMTran)target.Transactions.Cache.Insert();
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
                            target.Transactions.Cache.SetValueExt<PMTranExt.usrReassigned>(tran, true);

                            //Delete UsrPMCostReassignmentRunHistory
                            DeleteRelatedReassignmentRunHistory(pmTran);
                        }

                        target.Save.Press();
                        target.Document.Current = doc;

                        //Release
                        target.ReleaseDocument(target.Document.Current);
                        target.Save.Press();

                        ts.Complete();
                    }
                }

                target.Document.Current = PXSelect<PMRegister, Where<PMRegister.module, Equal<Current<PMRegister.module>>, And<PMRegister.origRefNbr, Equal<Current<PMRegister.refNbr>>>>>.Select(Base);
                throw new PXRedirectRequiredException(target, "Open Reversal");
            }
        }

        private void DeleteRelatedRecords(IEnumerable<PMTran> tranRows)
        {
            var usrReassignmentHistoryView = new PXSelect<UsrPMCostReassignmentHistory, Where<UsrPMCostReassignmentHistory.tranID, Equal<Required<UsrPMCostReassignmentHistory.tranID>>>>(Base);
            var usrReassignmentSourceTranView = new PXSelect<UsrPMCostReassignmentSourceTran, Where<UsrPMCostReassignmentSourceTran.tranID, Equal<Required<UsrPMCostReassignmentSourceTran.tranID>>>>(Base);
            var usrPMCostReassignmentPercentageView = new PXSelect<UsrPMCostReassignmentPercentage, Where<UsrPMCostReassignmentPercentage.tranID, Equal<Required<UsrPMCostReassignmentPercentage.tranID>>>>(Base);

            foreach (var tran in tranRows)
            {
                UsrPMCostReassignmentSourceTran reassignmentSourceTran = usrReassignmentSourceTranView.Select(tran.TranID).FirstOrDefault();
                if (reassignmentSourceTran == null) continue;

                using (new PXConnectionScope())
                {
                    using (var ts = new PXTransactionScope())
                    {
                        //Make source transaction available for reassignment again
                        PMTran sourceTran = PXSelect<PMTran, Where<PMTran.tranID, Equal<Required<PMTran.tranID>>>>.Select(Base, reassignmentSourceTran.SourceTranID);
                        if (sourceTran != null)
                        {
                            sourceTran.GetExtension<PMTranExt>().UsrReassigned = false;
                            Base.Caches<PMTran>().Update(sourceTran);
                        }

                        //Delete UsrPMCostReassignmentHistory
                        foreach (var usrReassignmentHistory in usrReassignmentHistoryView.Select(reassignmentSourceTran.TranID))
                        {
                            Base.Caches<UsrPMCostReassignmentHistory>().Delete((UsrPMCostReassignmentHistory)usrReassignmentHistory);
                        }

                        //Delete UsrPMCostReassignmentPercentage
                        foreach (var reassignmentPercentage in usrPMCostReassignmentPercentageView.Select(reassignmentSourceTran.TranID))
                        {
                            Base.Caches<UsrPMCostReassignmentPercentage>().Delete((UsrPMCostReassignmentPercentage)reassignmentPercentage);
                        }

                        //Delete UsrPMCostReassignmentRunHistory
                        DeleteRelatedReassignmentRunHistory(tran);

                        //Delete UsrPMCostReassignmentSourceTran
                        Base.Caches<UsrPMCostReassignmentSourceTran>().Delete(reassignmentSourceTran);

                        ts.Complete();
                    }
                }
            }
        }

        private void DeleteRelatedReassignmentRunHistory(PMTran tran)
        {
            var reassigmentView = new PXSelect<UsrPMCostReassignment, Where<UsrPMCostReassignment.pMReassignmentID, Equal<Required<UsrPMCostReassignment.pMReassignmentID>>>>(Base);
            var usrReassignmentSourceTranView = new PXSelect<UsrPMCostReassignmentSourceTran, Where<UsrPMCostReassignmentSourceTran.tranID, Equal<Required<UsrPMCostReassignmentSourceTran.tranID>>>>(Base);
            var usrPMCostReassignmentRunHistoryView = new PXSelect<UsrPMCostReassignmentRunHistory, Where<UsrPMCostReassignmentRunHistory.pMReassignmentID, Equal<Required<UsrPMCostReassignmentRunHistory.pMReassignmentID>>,
                                                                                                     And<UsrPMCostReassignmentRunHistory.revID, Equal<Required<UsrPMCostReassignmentRunHistory.revID>>,
                                                                                                     And2<Where<UsrPMCostReassignmentRunHistory.sourceTranID, Equal<Required<UsrPMCostReassignmentRunHistory.sourceTranID>>>,
                                                                                                             Or<UsrPMCostReassignmentRunHistory.destinationTranID, Equal<Required<UsrPMCostReassignmentRunHistory.destinationTranID>>>>>>>(Base);

            UsrPMCostReassignmentSourceTran reassignmentSourceTran = usrReassignmentSourceTranView.Select(tran.TranID).FirstOrDefault();
            if (reassignmentSourceTran != null)
            {
                //Get reassignmemt
                UsrPMCostReassignment reasignment = reassigmentView.Select(reassignmentSourceTran.PMReassignmentID);
                //Delete UsrPMCostReassignmentRunHistory
                var reassignmentRunHistory = usrPMCostReassignmentRunHistoryView.Select(reasignment.PMReassignmentID, reasignment.RevID, reassignmentSourceTran.TranID);
                if (reassignmentRunHistory != null)
                {
                    Base.Caches<UsrPMCostReassignmentRunHistory>().Delete((UsrPMCostReassignmentRunHistory)reassignmentRunHistory);
                    usrPMCostReassignmentRunHistoryView.Cache.Persist(PXDBOperation.Delete);
                }
            }
        }

        #endregion

    }
}