using PX.Data;

namespace ProjectCostReallocation.DAC
{
  public class PMRegisterExt : PXCacheExtension<PX.Objects.PM.PMRegister>
  {
      #region UsrIsReassignment
      [PXDBBool]
      [PXDefault(false)]
      [PXUIField(DisplayName="Document Reassigned", Visible = false)]
      public virtual bool? UsrIsReassignment { get; set; }
      public abstract class usrIsReassignment : IBqlField { }
      #endregion
  }
}