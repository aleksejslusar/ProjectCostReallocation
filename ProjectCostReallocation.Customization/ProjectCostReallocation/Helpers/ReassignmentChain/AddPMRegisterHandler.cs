using PX.Objects.GL;
using PX.Objects.PM;

namespace ProjectCostReallocation
{
    public class AddPMRegisterHandler : TransactionEntityBase
    {
        public AddPMRegisterHandler(ProjectCostReassignmentEntry reassignmentEntry, RegisterEntry registerEntry,
            PMCostReassigmentEntity entity) : base(reassignmentEntry, registerEntry, entity)
        {
            
        }

        protected override void HandlerRequest()
        {
            var pmRegisterRow = new PMRegister
            {
                Module = BatchModule.PM,
                Status = PMRegister.status.Balanced,
                Description = "Costs Reassignment " + Entity.PMReassignmentID
            };
            var newRegisterRow = RegisterEntry.Document.Insert(pmRegisterRow);
            RegisterEntry.Document.SetValueExt<PMRegisterExt.usrIsReassignment>(newRegisterRow, true);                  
        }
    }
}
