using IQP.Application.Contracts.AlgoTasks.Commands;
using IQP.Application.Contracts.AlgoTasks.Responses;
using IQP.Infrastructure.CodeRunner;

namespace IQP.Application.Services;

public interface IAlgoTasksService
{
   public Task<AlgoTaskResponse> CreateAlgoTask(CreateAlgoTaskCommand command);
   public Task<AlgoTaskResponse> UpdateAlgoTask(UpdateAlgoTaskCommand command);
   public Task<AlgoTaskResponse> AddNewLanguageToAlgoTask(AddNewLanguageToAlgoTaskCommand command);
   public Task<IEnumerable<AlgoTaskResponse>> GetAlgoTasks();
   public Task<AlgoTaskResponse> GetAlgoTaskById(Guid id);
   public Task<TestRun> RunTestsOnCode(RunTestsOnCodeCommand command);
   public Task<TestRun> TestAlgoTaskSolution(SubmitAlgoTaskSolutionCommand submissionCommand);
   public Task<TestRun> SubmitAlgoTaskSolution(SubmitAlgoTaskSolutionCommand submissionCommand);
}