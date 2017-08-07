using PX.Data;
using PX.Objects.CS;
using PX.Objects.GL;

namespace ProjectCostReallocation.DAC
{
  public class PMSetupExt : PXCacheExtension<PX.Objects.PM.PMSetup>
  {
      #region UsrReassignmentNumberingID

      [PXDBString(10)]
      [PXDefault("REASSIGNMT")]
      [PXUIField(DisplayName = "Reassignment Numbering Sequence")]
      [PXSelector(typeof (Numbering.numberingID), DescriptionField = typeof (Numbering.descr))]
      public virtual string UsrReassignmentNumberingID { get; set; }

      public abstract class usrReassignmentNumberingID : IBqlField
      {
      }

      #endregion
      #region UsrAutoReleaseReassignment

      [PXDBBool]
      [PXUIField(DisplayName = "Automatically Release Reassignment")]
      public virtual bool? UsrAutoReleaseReassignment { get; set; }

      public abstract class usrAutoReleaseReassignment : IBqlField
      {
      }

      #endregion
      #region UsrReassignmentAccountID

      public abstract class usrReassignmentAccountID : IBqlField
      {
      }

      [PXDefault]
      [Account(DisplayName = "Reassignment Accrual Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof (Account.description))]
      public virtual int? UsrReassignmentAccountID { get; set; }

      #endregion
      #region FAAccrualSubID

      public abstract class usrReassignmentSubID : IBqlField
      {
      }

      [PXDefault]
      [SubAccount(typeof (usrReassignmentAccountID), Visibility = PXUIVisibility.Visible, DisplayName = "Reassignment Accrual Subaccount", DescriptionField = typeof (Sub.description))]
      public virtual int? UsrReassignmentSubID { get; set; }

      #endregion

    }
}