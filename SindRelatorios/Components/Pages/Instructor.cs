using Microsoft.AspNetCore.Components;
using SindRelatorios.Application.Interfaces;
using InstructorEntity = SindRelatorios.Models.Entities.Instructor;

namespace SindRelatorios.Components.Pages;

public partial class Instructor
{
    [Inject]
    private IRepository<InstructorEntity> InstructorRepository { get; set; } = default!;

    [SupplyParameterFromForm]
    public InstructorEntity NewInstructor { get; set; } = new();

    private List<InstructorEntity>? _instructorsList;
    private string? _errorMessage;
    private string? _successMessage;

    protected override async Task OnInitializedAsync()
    {
        await LoadInstructors();
    }

    private async Task LoadInstructors()
    {
        try
        {
            var data = await InstructorRepository.GetAllAsync();
            _instructorsList = data.OrderBy(i => i.Name).ToList();
        }
        catch
        {
            _errorMessage = "Erro ao carregar lista de instrutores.";
        }
    }

    private async Task HandleAddInstructor()
    {
        _errorMessage = null;
        _successMessage = null;

        if (string.IsNullOrWhiteSpace(NewInstructor.Name))
        {
            _errorMessage = "O nome do instrutor é obrigatório.";
            return;
        }

        var finalName = NewInstructor.Name.Trim().ToUpper();

        if (_instructorsList != null && _instructorsList.Any(x => x.Name == finalName))
        {
            _errorMessage = $"O instrutor '{finalName}' já está cadastrado.";
            return;
        }

        try
        {
            NewInstructor.Name = finalName;
            await InstructorRepository.AddAsync(NewInstructor);

            _successMessage = "Instrutor cadastrado com sucesso!";
            NewInstructor = new InstructorEntity();
            await LoadInstructors();
        }
        catch
        {
            _errorMessage = "Erro ao salvar instrutor no banco de dados.";
        }
    }

    private async Task HandleDelete(Guid id)
    {
        _errorMessage = null;
        _successMessage = null;

        try
        {
            var success = await InstructorRepository.DeleteAsync(id);

            if (success)
            {
                var itemToRemove = _instructorsList?.FirstOrDefault(x => x.Id == id);
                if (itemToRemove != null)
                {
                    _instructorsList?.Remove(itemToRemove);
                }
                _successMessage = "Instrutor removido.";
            }
            else
            {
                _errorMessage = "Instrutor não encontrado.";
                await LoadInstructors();
            }
        }
        catch
        {
            _errorMessage = "Não é possível excluir este instrutor pois ele possui registros vinculados.";
        }
        
        StateHasChanged();
    }
}