using ProjectCostReallocation.DAC;
using PX.Data;

namespace ProjectCostReallocation.BL.ReassignmentChain
{
    public class WriteLogsDataHandler : TransactionEntityBase
    {
        public WriteLogsDataHandler(PMCostReassignmentProcessor processor, PMCostReassigmentEntity entity) : base(processor, entity)
        {
        }

        public override void ExecuteRequest()
        {
            foreach (var cachedValue in Processor.CachedValues)
            {
                WriteUsrPMCostReassignmentHistory(cachedValue.InsertedSourceTran.TranID, cachedValue.PMReassignmentID);
                WriteUsrPMCostReassignmentHistory(cachedValue.InsertedDestTran.TranID, cachedValue.PMReassignmentID);

                WriteUsrPMCostReassignmentSourceTran(cachedValue.PMReassignmentID, cachedValue.SourcePMTran.TranID, cachedValue.InsertedSourceTran.TranID);
                WriteUsrPMCostReassignmentSourceTran(cachedValue.PMReassignmentID, cachedValue.SourcePMTran.TranID, cachedValue.InsertedDestTran.TranID);

                WriteUsrPMCostReassignmentPercentage(cachedValue.PMReassignmentID, cachedValue.InsertedDestTran.TranID, cachedValue.ReassigmentValue.Selection, cachedValue.ReassigmentValue.Percentage, cachedValue.ReassigmentValue.SourceAmount, cachedValue.ReassigmentValue.DestAmount);

                WriteUsrPMCostReassignmentRunHistory(cachedValue.PMReassignmentID, cachedValue.RevID, cachedValue.InsertedSourceTran.TaskID, cachedValue.InsertedSourceTran.TranID, cachedValue.InsertedDestTran.TaskID, cachedValue.InsertedDestTran.TranID);
            }

            Processor.ReassignmentHistoryView.Cache.Persist(PXDBOperation.Insert);
            Processor.ReassignmentSourceTranView.Cache.Persist(PXDBOperation.Insert);
            Processor.ReassignmentPercentageView.Cache.Persist(PXDBOperation.Insert);
            Processor.ReassignmentRunHistoryView.Cache.Persist(PXDBOperation.Insert);


            // Call next chain
            Successor?.ExecuteRequest();
        }

        #region Private
        private void WriteUsrPMCostReassignmentHistory(long? pmTranID, string pmReassignmentID)
        {
            var historyRow = new UsrPMCostReassignmentHistory
            {
                PMReassignmentID = pmReassignmentID,
                TranID = pmTranID
            };

            Processor.ReassignmentHistoryView.Insert(historyRow);
        }

        private void WriteUsrPMCostReassignmentSourceTran(string reassignmentId, long? sourceTranID, long? destTranID)
        {
            var row = new UsrPMCostReassignmentSourceTran
            {
                PMReassignmentID = reassignmentId,
                SourceTranID = sourceTranID,
                TranID = destTranID
            };

            Processor.ReassignmentSourceTranView.Insert(row);
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

            Processor.ReassignmentPercentageView.Insert(row);
        }

        private void WriteUsrPMCostReassignmentRunHistory(string reassignmentId, int? revId, int? sourceTaskId, long? sourceTranId, int? destTaskId, long? destTranId)
        {
            var row = new UsrPMCostReassignmentRunHistory
            {
                PMReassignmentID = reassignmentId,
                RevID = revId,
                SourceTaskID = sourceTaskId,
                SourceTranID = sourceTranId,
                DestinationTaskID = destTaskId,
                DestinationTranID = destTranId
            };

            Processor.ReassignmentRunHistoryView.Insert(row);
        } 
        #endregion
    }
}
