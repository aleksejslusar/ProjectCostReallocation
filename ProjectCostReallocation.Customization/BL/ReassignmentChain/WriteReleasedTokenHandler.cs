using ProjectCostReallocation.DAC;
using PX.Data;
using PX.Objects.PM;

namespace ProjectCostReallocation.BL.ReassignmentChain
{
    public class WriteReleasedTokenHandler : TransactionEntityBase
    {
        private readonly RegisterEntry _registerGraph;

        public WriteReleasedTokenHandler(PMCostReassignmentProcessor processor, PMCostReassigmentEntity entity) : base(processor, entity)
        {
            _registerGraph = Processor.RegisterGraph;
        }

        public override void ExecuteRequest()
        {
            ReleasePMRegister();
            
            // Call next chain
            Successor?.ExecuteRequest();
        }

        private void ReleasePMRegister()
        {
            PMSetup setup = _registerGraph.Setup.Select();
            var value = setup.GetExtension<PMSetupExt>()?.UsrAutoReleaseReassignment.GetValueOrDefault();
            if (value == true)
            {

                if (_registerGraph.Document.Current != null)
                {
                    _registerGraph.Actions.PressSave();
                    _registerGraph.ReleaseDocument(_registerGraph.Document.Current);
                }
            }
        }

    }
}
