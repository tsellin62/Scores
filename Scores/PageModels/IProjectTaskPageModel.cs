using CommunityToolkit.Mvvm.Input;
using Scores.Models;

namespace Scores.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}