using PX.Data;

namespace ProjectCostReallocation
{
  public class PMTranExt : PXCacheExtension<PX.Objects.PM.PMTran>
  {
      #region UsrReassigned
      [PXDBBool]
      [PXDefault(false)]
      [PXUIField(DisplayName="Transaction Reassigned")]

      public virtual bool? UsrReassigned { get; set; }
      public abstract class usrReassigned : IBqlField { }
      #endregion
  }
}