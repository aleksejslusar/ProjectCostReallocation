using PX.Objects.PM;

namespace ProjectCostReallocation
{
    public class CachedValue
    {
        public CachedValue(PMTran sourcePMTran, PMTran insertedSourceTran, PMTran insertedDestTran, ReassigmentValue value, string pmReassignmentID, int? revID)
        {
            SourcePMTran = sourcePMTran;
            InsertedSourceTran = insertedSourceTran;
            InsertedDestTran = insertedDestTran;
            ReassigmentValue = value;
            PMReassignmentID = pmReassignmentID;
            RevID = revID;
        }

        public PMTran SourcePMTran { get; set; }
        public PMTran InsertedSourceTran { get; }
        public PMTran InsertedDestTran { get; }
        public ReassigmentValue ReassigmentValue { get; }
        public string  PMReassignmentID { get; }
        public int? RevID { get; set; }
    }
}