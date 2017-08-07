namespace ProjectCostReallocation
{
    public static class ProjectCostReassignmentMessages
    {
        public const string REASSIGNMENT_SUCCESS = "Project cost reassignment operation was successful";
        public const string REASSIGNMENT_NOT_SUCCESS = "Project cost reassignment operation(s) was unsuccessful, at least one project hasn't been processed";
        public const string REASSAIMENT_AMOUNT_ERROR_UNITS = "Reassignment amount cannot be calculated. Reassignment selection value for the processed reassignment equals 0. Check Number of Units.";
        public const string REASSAIMENT_AMOUNT_ERROR_SF = "Reassignment amount cannot be calculated. Reassignment selection value for the processed reassignment equals 0. Check Square Footage.";
        public const string REASSAIMENT_AMOUNT_ERROR = "Calculated value of reassignment amount is incorrect.";
        public const string REASSAIMENT_NOTHING_TO_REASSIGN_TRAN_NOT_FOUND = "Nothing to reassign: Transactions for source reassignment task not found";
        public const string REASSAIMENT_NOTHING_TO_REASSIGN_TRAN_REQASSIGNED_EARLIER = "Nothing to reassign: Transactions for source reassignment task was reassigned earlier";    
        public const string REASSIGNMENT_REVERSE_ERROR = "PM Error: Reversal for the given reassignment already exists. Reassignment can be reversed only once. RefNbr of the reversal document is {0}.";

        public const string REASSIGNMENT_NUMBERING_SEQUENCE_VALUE_NOT_CONFIGURED = "Reassignment Numbering Sequence value should be configured. Use screen PM101000 for configure Reassignment Numbering Sequence value.";
        public const string SAME_PROJECT_TASK_CANNOT_BE_USED_AS_A_SOURCE_AND_AS_A_DESTINATION_TASK_IN_A_SINGLE_REASSIGNMENT = "The same Project Task cannot be used as a Source and as a Destination Task in a single Reassignment.";
        public const string SQUARE_FOOTAGE_VALUE_CANNOT_BE_NEGATIVE = "Square Footage value cannot be negative.";
        public const string UNIT_COUNT_VALUE_CANNOT_BE_NEGATIVE = "Unit Count value cannot be negative.";
        public const string SOURCE_AND_DESTINATION_TOTALS_MUST_BE_BALANCED_BEFORE_ACTIVATING_THE_REASSIGNMENT = "At least one of the Reassignment Totals must be greater than 0.";
        public const string PROJECT_TASK_ALREADY_USED = "Project Task <{0}> is used in the following Reassignment(s): <{1}>. The update may cause source-destination disbalance in given reassignment(s)";
    }
}