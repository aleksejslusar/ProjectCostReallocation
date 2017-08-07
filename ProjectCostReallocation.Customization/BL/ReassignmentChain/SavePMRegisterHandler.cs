using System.Linq;
using ProjectCostReallocation.DAC;
using PX.Objects.PM;

namespace ProjectCostReallocation.BL.ReassignmentChain
{
    public class SavePMRegisterHandler: TransactionEntityBase
    {
        private readonly RegisterEntry _registerGraph;

        public SavePMRegisterHandler(PMCostReassignmentProcessor processor, PMCostReassigmentEntity entity) : base(processor, entity)
        {
            _registerGraph = Processor.RegisterGraph;
        }

        public override void ExecuteRequest()
        {
            if (_registerGraph.Document.Current != null && _registerGraph.Transactions.Select().Any())
            {
                _registerGraph.Actions.PressSave();
            }

            // Call next chain
            Successor?.ExecuteRequest();
        }
    }
}
