using System;
using System.Collections.Generic;
using System.Linq;
using ProjectCostReallocation.BL.ReassignmentChain;
using ProjectCostReallocation.DAC;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.PM;

namespace ProjectCostReallocation.BL
{
    public class PMCostReassignmentProcessor: IDisposable
    {
        public readonly RegisterEntry RegisterGraph;
        public readonly ProjectCostReassignmentEntry ReassignmentGraph;
        public readonly PXGraph ProcessingGraph;
        public readonly PXSelectBase<UsrPMCostReassignmentHistory> ReassignmentHistoryView;
        public readonly PXSelectBase<UsrPMCostReassignmentSourceTran> ReassignmentSourceTranView;
        public readonly PXSelectBase<UsrPMCostReassignmentPercentage> ReassignmentPercentageView;
        public readonly PXSelectBase<UsrPMCostReassignmentRunHistory> ReassignmentRunHistoryView;
        public readonly PXSelectBase<UsrPMCostReassignmentSourceTran> ProcessedTransactions;       

        public readonly List<PMTran> ReassignedSourcePMTrans = new List<PMTran>();
        public readonly List<CachedValue> CachedValues = new List<CachedValue>();
        public PMSetup Setup;        

        public PMCostReassignmentProcessor(RegisterEntry registerGraph, ProjectCostReassignmentEntry reassignmentGraph)
        {            
            RegisterGraph = registerGraph;
            ReassignmentGraph = reassignmentGraph;
            ProcessingGraph = new PXGraph();
            //Init selects

            ReassignmentHistoryView = new PXSelect<UsrPMCostReassignmentHistory>(ProcessingGraph);
            ReassignmentSourceTranView = new PXSelect<UsrPMCostReassignmentSourceTran>(ProcessingGraph);
            ReassignmentPercentageView = new PXSelect<UsrPMCostReassignmentPercentage>(ProcessingGraph);
            ReassignmentRunHistoryView = new PXSelect<UsrPMCostReassignmentRunHistory, 
                                                Where<UsrPMCostReassignmentRunHistory.pMReassignmentID, Equal<Required<UsrPMCostReassignmentRunHistory.pMReassignmentID>>>>(ProcessingGraph);
            ProcessedTransactions = new PXSelectJoin<UsrPMCostReassignmentSourceTran,
                                               InnerJoin<UsrPMCostReassignmentRunHistory, On<UsrPMCostReassignmentRunHistory.sourceTranID, Equal<UsrPMCostReassignmentSourceTran.tranID>>,
                                               InnerJoin<PMTran, On<PMTran.tranID, Equal<UsrPMCostReassignmentRunHistory.destinationTranID>>>>,
                                                    Where<UsrPMCostReassignmentSourceTran.sourceTranID, Equal<Required<UsrPMCostReassignmentSourceTran.sourceTranID>>,
                                                      And<UsrPMCostReassignmentRunHistory.pMReassignmentID, Equal<Required<UsrPMCostReassignmentRunHistory.pMReassignmentID>>,
                                                      And<UsrPMCostReassignmentRunHistory.revID, Equal<UsrPMCostReassignmentRunHistory.revID>>>>>(ProcessingGraph);           
        }

        #region Public

        public void ProcessReassigment(PMCostReassigmentEntity entity)
        {
            ReassignmentGraph.PMCostReassignment.Current = ReassignmentGraph.PMCostReassignment.Search<UsrPMCostReassignment.pMReassignmentID>(entity.PMReassignmentID);
            Setup = ReassignmentGraph.PMSetupSelect.Select().FirstOrDefault();

            using (var ts = new PXTransactionScope())
            {
                // === Transaction scope ===
                //1. Create PMRegister
                //2. Process each reassignmentEntity row and create transactions
                //3. Checking whether to save or not. Save changes
                //4. Write data information to history and log tables
                //5. Mark source transations as reassigned
                //6. Release PMRegister
                
                var insertPMRegister = new AddPMRegisterHandler(this, entity);
                var insertPMTrans = new AddPMTransHandler(this, entity);
                var savePMRegister = new SavePMRegisterHandler(this, entity);
                var writeLogData = new WriteLogsDataHandler(this, entity);
                var writeReassignedToken = new WriteReassignedTokenHandler(this, entity);
                var writeReleasedToken = new WriteReleasedTokenHandler(this, entity);

           /*1*/ insertPMRegister.Successor = insertPMTrans;
           /*2*/ insertPMTrans.Successor = savePMRegister;
           /*3*/ savePMRegister.Successor = writeLogData;
           /*4*/ writeLogData.Successor = writeReassignedToken;
           /*5*/ writeReassignedToken.Successor = writeReleasedToken;
           /*6*/ 
                insertPMRegister.ExecuteRequest();

                ts.Complete();               
            }
        }

        public void Dispose()
        {
            ProcessingGraph.Clear();
            ReassignmentGraph.Clear();
            ReassignmentHistoryView.Cache.Clear();
            ReassignmentSourceTranView.Cache.Clear();
            ReassignmentPercentageView.Cache.Clear();
        }

        #endregion
                       
    }
}
