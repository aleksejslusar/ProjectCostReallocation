using System;

namespace ProjectCostReallocation.DAC
{
    public interface IUsrPMCostReassignmentProjectAndTask
    {
        string PMReassignmentID { get; set; }
        int? LineID { get; set; }
        int? ProjectID { get; set; }
        int? TaskID { get; set; }        
        Guid? NoteID { get; set; }
        Guid? CreatedByID { get; set; }
        byte[] tstamp { get; set; }
        string CreatedByScreenID { get; set; }
        DateTime? CreatedDateTime { get; set; }
        Guid? LastModifiedByID { get; set; }
        DateTime? LastModifiedDateTime { get; set; }
        string LastModifiedByScreenID { get; set; }        
    }
}