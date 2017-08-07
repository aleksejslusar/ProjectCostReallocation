using ProjectCostReallocation.DAC;

namespace ProjectCostReallocation.BL.ReassignmentChain
{
    public abstract class TransactionEntityBase
    {
        protected readonly PMCostReassigmentEntity entity;

        protected TransactionEntityBase(PMCostReassignmentProcessor processor, PMCostReassigmentEntity entity)
        {
            this.entity = entity;
            Processor = processor;
        }

        public TransactionEntityBase Successor { get; set; }
        protected PMCostReassignmentProcessor Processor { get; }
        public abstract void ExecuteRequest();
    }
}
