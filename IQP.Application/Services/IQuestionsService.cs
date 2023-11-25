using IQP.Application.Contracts.Questions.Commands;
using IQP.Application.Contracts.Questions.Responses;

namespace IQP.Application.Services;

public interface IQuestionsService
{
    public Task<QuestionResponse> CreateQuestion(CreateQuestionCommand command);
    public Task<IEnumerable<QuestionResponse>> GetQuestions();
    public Task<QuestionResponse> GetQuestionById(Guid id);
    public Task<QuestionResponse> UpdateQuestion(UpdateQuestionCommand command);
    public Task<QuestionResponse> DeleteQuestion(Guid id);
}