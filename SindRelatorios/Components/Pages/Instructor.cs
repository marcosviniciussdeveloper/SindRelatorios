using Microsoft.AspNetCore.Components;
using SindRelatorios.Application.Interfaces;
using SindRelatorios.Models.Entities;

// Alias para evitar conflito com o nome da Página
using InstructorEntity = SindRelatorios.Models.Entities.Instructor;

namespace SindRelatorios.Components.Pages;

public partial class Instructor
{
    [Inject]
    private IRepository<InstructorEntity> InstructorRepository { get; set; } = default!;

    [SupplyParameterFromForm]
    public InstructorEntity NewInstructor { get; set; } = new();

    // Propriedades acessíveis pela View
    protected List<InstructorEntity> InstructorsList { get; set; } = new();
    protected string SearchTerm { get; set; } = "";
    protected string _message;
    protected string _messageType;

    // Propriedade computada para o filtro (Lógica de UI)
    protected List<InstructorEntity> FilteredList =>
        string.IsNullOrWhiteSpace(SearchTerm)
            ? InstructorsList
            : InstructorsList.Where(i => i.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        try
        {
            var data = await InstructorRepository.GetAllAsync();
            InstructorsList = data.OrderBy(x => x.Name).ToList();
        }
        catch
        {
            _message = "Erro ao carregar dados.";
            _messageType = "danger";
        }
    }

    private async Task HandleAddInstructor()
    {
        _message = null;

        if (string.IsNullOrWhiteSpace(NewInstructor.Name)) return;

        // Validação de regra de negócio simples
        if (InstructorsList.Any(x => x.Name.Equals(NewInstructor.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            _message = "Este instrutor já está cadastrado.";
            _messageType = "danger";
            return;
        }

        try
        {
            NewInstructor.Name = NewInstructor.Name.Trim().ToUpper();
            await InstructorRepository.AddAsync(NewInstructor);

            _message = "Cadastrado com sucesso!";
            _messageType = "success";
            NewInstructor = new InstructorEntity(); // Limpa form
            await LoadData();
        }
        catch
        {
            _message = "Erro ao salvar no banco.";
            _messageType = "danger";
        }
    }

    private async Task HandleDelete(Guid id)
    {
        try
        {
            var success = await InstructorRepository.DeleteAsync(id);
            if (success)
            {
                var item = InstructorsList.FirstOrDefault(x => x.Id == id);
                if (item != null) InstructorsList.Remove(item);
            }
            else
            {
                await LoadData();
            }
        }
        catch
        {
            _message = "Não é possível excluir instrutor com aulas vinculadas.";
            _messageType = "danger";
        }
    }

    protected string GetInitials(string name)
    {
        if (string.IsNullOrEmpty(name)) return "?";
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1) return parts[0][0].ToString();
        return $"{parts[0][0]}{parts[parts.Length - 1][0]}";
    }
}