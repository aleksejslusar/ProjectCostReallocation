using PX.Data;
using PX.Objects.PM;

namespace ProjectCostReallocation
{
    public class WriteReleasedTokenHandler : TransactionEntityBase
    {
        public WriteReleasedTokenHandler(ProjectCostReassignmentEntry reassignmentEntry, RegisterEntry registerEntry, PMCostReassigmentEntity entity) : base(reassignmentEntry, registerEntry, entity)
        {
        }

        protected override void HandlerRequest()
        {
            ReleasePMRegister();                       
        }

        private void ReleasePMRegister()
        {
            PMSetup setup = RegisterEntry.Setup.Select();
            var value = PXCache<PMSetup>.GetExtension<PMSetupExt>(setup)?.UsrAutoReleaseReassignment.GetValueOrDefault();
            if (value == true)
            {

                if (RegisterEntry.Document.Current != null)
                {
                    RegisterEntry.Actions.PressSave();
                    RegisterEntry.ReleaseDocument(RegisterEntry.Document.Current);
                }
            }
        }

    }
}
