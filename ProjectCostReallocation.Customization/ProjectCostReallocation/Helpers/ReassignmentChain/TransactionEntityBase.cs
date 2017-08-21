using System;
using System.Collections.Generic;
using PX.Objects.PM;

namespace ProjectCostReallocation
{
    public abstract class TransactionEntityBase : IDisposable
    {
        protected readonly ProjectCostReassignmentEntry ReassignmentEntry;
        protected readonly RegisterEntry RegisterEntry;
        protected PMCostReassigmentEntity Entity;

        public static readonly List<PMTran> ReassignedSourcePMTrans = new List<PMTran>();
        public static readonly List<CachedValue> CachedValues = new List<CachedValue>();

        protected TransactionEntityBase(ProjectCostReassignmentEntry reassignmentEntry, RegisterEntry registerEntry, PMCostReassigmentEntity entity)
        {
            ReassignmentEntry = reassignmentEntry;
            RegisterEntry = registerEntry;
            Entity = entity;
        }

        public TransactionEntityBase Successor { get; set; }
        
        protected abstract void HandlerRequest();

        public void ExecuteTransaction()
        {
            //Call andler owweriden method
            HandlerRequest();
                        
            // Call next chain if exist
            Successor?.ExecuteTransaction();
        }

        public void Dispose()
        {
            ReassignedSourcePMTrans.Clear();
            CachedValues.Clear();
            Entity = null;
        }
    }
}
