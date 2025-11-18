using Microsoft.AspNetCore.Components;
using SindRelatorios.Application.Interfaces;
using InstructorEntity = SindRelatorios.Models.Entities.Instructor;

namespace SindRelatorios.Components.Pages;

public partial class Instructor
{
    [Inject]
    private IRepository<InstructorEntity> InstructorRepository { get; set; } = default!;

    private List<InstructorEntity>? instructors;
    private InstructorEntity newInstructor = new();
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        await LoadInstructors();
    }

    private async Task LoadInstructors()
    {
        try
        {
            var list = await InstructorRepository.GetAllAsync();
            instructors = list.OrderBy(i => i.Name).ToList();
        }
        catch (Exception ex)
        {
            errorMessage = "Erro ao carregar instrutores.";
            Console.WriteLine(ex);
        }
    }

    private async Task HandleAddInstructor()
    {
        errorMessage = null;

        if (string.IsNullOrWhiteSpace(newInstructor.Name))
        {
            errorMessage = "O nome do instrutor é obrigatório.";
            return;
        }

        try
        {
            await InstructorRepository.AddAsync(newInstructor);

            newInstructor = new InstructorEntity();
            await LoadInstructors();
        }
        catch (Exception ex)
        {
            errorMessage = "Erro ao salvar instrutor.";
            Console.WriteLine(ex);
        }
    }

    private async Task HandleDelete(Guid id)
    {
        errorMessage = null;

        try
        {
            await InstructorRepository.DeleteAsync(id);
            await LoadInstructors();
        }
        catch (Exception ex)
        {
            errorMessage = "Erro ao excluir instrutor.";
            Console.WriteLine(ex);
        }
    }
}