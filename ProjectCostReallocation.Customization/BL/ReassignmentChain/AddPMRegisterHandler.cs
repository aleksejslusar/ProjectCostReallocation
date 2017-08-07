using ProjectCostReallocation.DAC;
using PX.Objects.GL;
using PX.Objects.PM;

namespace ProjectCostReallocation.BL.ReassignmentChain
{
    public class AddPMRegisterHandler : TransactionEntityBase
    {
        private readonly RegisterEntry _registerGraph;

        public AddPMRegisterHandler(PMCostReassignmentProcessor processor, PMCostReassigmentEntity entity): base(processor, entity)
        {
            _registerGraph = Processor.RegisterGraph;
        }

        public override void ExecuteRequest()
        {
            var pmRegisterRow = new PMRegister
            {
                Module = BatchModule.PM,
                Status = PMRegister.status.Balanced,
                Description = "Costs Reassignment " + entity.PMReassignmentID
            };
            var newRegisterRow = _registerGraph.Document.Insert(pmRegisterRow);
            _registerGraph.Document.Cache.SetValueExt<PMRegisterExt.usrIsReassignment>(newRegisterRow, true);
            Successor?.ExecuteRequest();
        }
    }
}
