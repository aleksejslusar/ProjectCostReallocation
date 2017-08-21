using PX.Objects.PM;

namespace ProjectCostReallocation
{
    public class WriteLogsDataHandler : TransactionEntityBase
    {
        public WriteLogsDataHandler(ProjectCostReassignmentEntry reassignmentEntry, RegisterEntry registerEntry, PMCostReassigmentEntity entity) : base(reassignmentEntry, registerEntry, entity)
        {
        }

        protected override void HandlerRequest()
        {
            foreach (var cachedValue in CachedValues)
            {
                WriteUsrPMCostReassignmentHistory(cachedValue.InsertedSourceTran.TranID, cachedValue.PMReassignmentID);
                WriteUsrPMCostReassignmentHistory(cachedValue.InsertedDestTran.TranID, cachedValue.PMReassignmentID);

                WriteUsrPMCostReassignmentSourceTran(cachedValue.PMReassignmentID, cachedValue.SourcePMTran.TranID, cachedValue.InsertedSourceTran.TranID);
                WriteUsrPMCostReassignmentSourceTran(cachedValue.PMReassignmentID, cachedValue.SourcePMTran.TranID, cachedValue.InsertedDestTran.TranID);

                WriteUsrPMCostReassignmentPercentage(cachedValue.PMReassignmentID, cachedValue.InsertedDestTran.TranID, cachedValue.ReassigmentValue.Selection, cachedValue.ReassigmentValue.Percentage, cachedValue.ReassigmentValue.SourceAmount, cachedValue.ReassigmentValue.DestAmount);

                WriteUsrPMCostReassignmentRunHistory(cachedValue.PMReassignmentID, cachedValue.RevID, cachedValue.InsertedSourceTran.TaskID, cachedValue.InsertedSourceTran.TranID, cachedValue.InsertedDestTran.TaskID, cachedValue.InsertedDestTran.TranID);
            }

            ReassignmentEntry.Actions.PressSave();
        }

        #region Private
        private void WriteUsrPMCostReassignmentHistory(long? pmTranID, string pmReassignmentID)
        {
            var historyRow = new UsrPMCostReassignmentHistory
            {
                PMReassignmentID = pmReassignmentID,
                TranID = pmTranID
            };

            ReassignmentEntry.ReassignmentHistoryView.Insert(historyRow);
        }

        private void WriteUsrPMCostReassignmentSourceTran(string reassignmentId, long? sourceTranID, long? destTranID)
        {
            var row = new UsrPMCostReassignmentSourceTran
            {
                PMReassignmentID = reassignmentId,
                SourceTranID = sourceTranID,
                TranID = destTranID
            };

            ReassignmentEntry.ReassignmentSourceTranView.Insert(row);
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

            ReassignmentEntry.ReassignmentPercentageView.Insert(row);
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

            ReassignmentEntry.ReassignmentRunHistoryView.Insert(row);
        } 
        #endregion
    }
}
