using System.Linq;
using PX.Objects.PM;

namespace ProjectCostReallocation
{
    public class SavePMRegisterHandler: TransactionEntityBase
    {
        public SavePMRegisterHandler(ProjectCostReassignmentEntry reassignmentEntry, RegisterEntry registerEntry, PMCostReassigmentEntity entity) : base(reassignmentEntry, registerEntry, entity)
        {
        }

        protected override void HandlerRequest()
        {
            if (RegisterEntry.Document.Current != null && RegisterEntry.Transactions.Select().Any())
            {
                RegisterEntry.Actions.PressSave();
            }
            else
            {
                Successor = null; //Exit chain
            }
        }
    }
}
