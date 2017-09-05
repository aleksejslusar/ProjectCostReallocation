using PX.Data;
using PX.Objects.PM;

namespace ProjectCostReallocation.ProjectCostReallocation.Helpers
{
    public class PMCostReassignmentViewer
    {
        public static void ViewProjectCommon(int? currentProjectID)
        {
            var graph = PXGraph.CreateInstance<ProjectEntry>();
            graph.Project.Current = graph.Project.Search<PMProject.contractID>(currentProjectID);
            if (graph.Project.Current != null)
            {
                throw new PXRedirectRequiredException(graph, true, Messages.ViewProject) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
            }
        }

        public static void ViewTaskCommon(int? currentTaskID)
        {
            var graph = PXGraph.CreateInstance<ProjectTaskEntry>();
            graph.Task.Current = graph.Task.Search<PMTask.taskID>(currentTaskID);
            throw new PXRedirectRequiredException(graph, true, Messages.ViewTask) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
        }
    }
}
