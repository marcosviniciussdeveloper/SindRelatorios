using Microsoft.AspNetCore.Components;
using SindRelatorios.Application.Interfaces;
using SindRelatorios.Models.Entities;

using InstructorEntity = SindRelatorios.Models.Entities.Instructor;

namespace SindRelatorios.Components.Pages;

public partial class Instructor
{
    [Inject] private IRepository<InstructorEntity> InstructorRepository { get; set; } = default!;

    // Variável exclusiva para o Formulário de CRIAÇÃO (Esquerda)
    [SupplyParameterFromForm]
    public InstructorEntity NewInstructor { get; set; } = new();

    // Variável exclusiva para o MODAL DE EDIÇÃO
    public InstructorEntity EditingInstructor { get; set; } = new();

    // Controle de dados e visual
    protected List<InstructorEntity> InstructorsList { get; set; } = new();
    protected string SearchTerm { get; set; } = "";
    protected string _message;
    protected string _messageType;
    
    // Controle do Modal
    protected bool _showEditModal = false;

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
        var data = await InstructorRepository.GetAllAsync();
        InstructorsList = data.OrderBy(x => x.Name).ToList();
    }

    // --- LÓGICA DE CRIAÇÃO (SEMPRE NOVO) ---
    private async Task HandleCreate()
    {
        _message = null;
        if (string.IsNullOrWhiteSpace(NewInstructor.Name)) return;

        // Verifica duplicidade
        if (InstructorsList.Any(x => x.Name.Equals(NewInstructor.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            _message = "Este instrutor já existe.";
            _messageType = "danger";
            return;
        }

        try
        {
            NewInstructor.Name = NewInstructor.Name.Trim().ToUpper();
            await InstructorRepository.AddAsync(NewInstructor);

            _message = "Instrutor cadastrado!";
            _messageType = "success";
            NewInstructor = new InstructorEntity(); // Limpa apenas o formulário de criação
            await LoadData();
        }
        catch
        {
            _message = "Erro ao salvar.";
            _messageType = "danger";
        }
    }

    // --- LÓGICA DE EDIÇÃO (MODAL) ---
    
    // 1. Abre o Modal e Copia os dados
    protected void OpenEditModal(InstructorEntity instructor)
    {
        // Copia os dados para não editar a lista em tempo real (Clone manual)
        EditingInstructor = new InstructorEntity 
        { 
            Id = instructor.Id, 
            Name = instructor.Name 
        };
        _showEditModal = true;
    }

    // 2. Fecha o Modal
    protected void CloseEditModal()
    {
        _showEditModal = false;
        EditingInstructor = new InstructorEntity();
    }

    // 3. Salva a Edição
    protected async Task HandleUpdate()
    {
        if (string.IsNullOrWhiteSpace(EditingInstructor.Name)) return;

        try
        {
            EditingInstructor.Name = EditingInstructor.Name.Trim().ToUpper();
            await InstructorRepository.UpdateAsync(EditingInstructor);
            
            await LoadData();
            CloseEditModal(); // Fecha a janela
        }
        catch
        {
            // Em produção, use um Toast/Alert. Aqui vou apenas fechar para simplificar.
            Console.WriteLine("Erro ao atualizar");
        }
    }

    // --- LÓGICA DE EXCLUSÃO ---
    private async Task HandleDelete(Guid id)
    {
        try
        {
            await InstructorRepository.DeleteAsync(id);
            
            // Remove visualmente
            var item = InstructorsList.FirstOrDefault(x => x.Id == id);
            if (item != null) InstructorsList.Remove(item);
        }
        catch
        {
            _message = "Não é possível excluir (pode ter vínculos).";
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