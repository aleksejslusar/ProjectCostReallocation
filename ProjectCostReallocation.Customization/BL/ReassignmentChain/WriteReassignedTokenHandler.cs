using System.Collections.Generic;
using ProjectCostReallocation.DAC;
using PX.Data;
using PX.Objects.PM;

namespace ProjectCostReallocation.BL.ReassignmentChain
{
    public class WriteReassignedTokenHandler: TransactionEntityBase
    {
        private readonly RegisterEntry _registerGraph;

        public WriteReassignedTokenHandler(PMCostReassignmentProcessor processor, PMCostReassigmentEntity entity) : base(processor, entity)
        {
            _registerGraph = Processor.RegisterGraph;
        }

        public override void ExecuteRequest()
        {
            UpdatePMTranUsrReassigned(Processor.ReassignedSourcePMTrans);
            
            // Call next chain
            Successor?.ExecuteRequest();
        }

        private void UpdatePMTranUsrReassigned(IEnumerable<PMTran> sourceTrans)
        {
            foreach (var sourceTran in sourceTrans)
            {
                sourceTran.GetExtension<PMTranExt>().UsrReassigned = true;
                _registerGraph.Transactions.Update(sourceTran);
                _registerGraph.Persist(typeof(PMTran), PXDBOperation.Update);
            }
        }
    }
}
