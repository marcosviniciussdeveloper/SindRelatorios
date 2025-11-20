using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore; // Necessário para AsNoTracking
using SindRelatorios.Application.Interfaces;
using SindRelatorios.Models.Entities;
using SindRelatorios.Models.Entities.Enums;
using InstructorEntity = SindRelatorios.Models.Entities.Instructor;

namespace SindRelatorios.Components.Pages;

public partial class DetailsScale
{
    [Parameter] public Guid Id { get; set; }

    [Inject] private IOpeningRepository OpeningRepo { get; set; } = default!;
    [Inject] private IRepository<InstructorEntity> InstructorRepo { get; set; } = default!; 
    [Inject] private NavigationManager Nav { get; set; } = default!;

    private OpeningCalendar? Opening { get; set; }
    
    // Lista de referência apenas para leitura (Consulta)
    private List<InstructorEntity> AllInstructors { get; set; } = new();
    
    private string? _errorMessage;
    private bool _isSaving = false;

    protected override async Task OnInitializedAsync()
    {
        try
        {
           
            var data = await InstructorRepo.GetAllAsync();
            AllInstructors = data.OrderBy(x => x.Name).ToList();

            // 2. Carrega a Escala com os Slots
            Opening = await OpeningRepo.GetByIdWithDetailsAsync(Id);

           
            if (Opening != null)
            {
                foreach (var slot in Opening.Slots)
                {
                    if (slot.InstructorId.HasValue)
                    {
                        var instr = AllInstructors.FirstOrDefault(i => i.Id == slot.InstructorId);
                        if (instr != null)
                        {
                            slot.Instructor = instr; 
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _errorMessage = $"Erro ao carregar dados: {ex.Message}";
        }
    }

    private void AddSlot(bool isExtra)
    {
        if (Opening == null) return;
        
        Opening.Slots.Add(new OpeningSlot
        {
            Id = Guid.Empty, // Garante que é novo (Insert)
            OpeningCalendarId = Opening.Id,
            Status = SlotStatus.Planejado,
            Shift = "NOITE",
            IsExtra = isExtra,
            Instructor = null,
            InstructorId = null
        });
    }

    private void UpdateSlotInstructor(OpeningSlot slot, string instructorName)
    {
        try
        {
            _errorMessage = null;
            
            var instr = AllInstructors.FirstOrDefault(i => i.Name.Equals(instructorName, StringComparison.OrdinalIgnoreCase));
            
            if (instr != null)
            {
                slot.InstructorId = instr.Id;
                slot.Instructor = instr; // Atualiza visualmente
            }
            else
            {
                slot.InstructorId = null;
                slot.Instructor = null;
            }
        }
        catch (Exception ex)
        {
            _errorMessage = $"Erro na seleção: {ex.Message}";
        }
    }

    private void RemoveSlot(OpeningSlot slot)
    {
        Opening?.Slots.Remove(slot);
    }

    private async Task HandleSave()
    {
        if (Opening == null) return;

        _isSaving = true;
        _errorMessage = null;

        try
        {
            
            foreach (var slot in Opening.Slots)
            {
                if (slot.InstructorId.HasValue && slot.InstructorId != Guid.Empty)
                {
                    slot.Instructor = null; 
                }
                else
                {
                    slot.InstructorId = null;
                    slot.Instructor = null;
                }
            }

            Opening.Date = DateTime.SpecifyKind(Opening.Date, DateTimeKind.Utc);

            await OpeningRepo.UpdateAsync(Opening);
            
            Nav.NavigateTo("escalas");
        }
        catch (Exception ex)
        {
            var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            _errorMessage = $"Erro ao salvar: {msg}";
            await OnInitializedAsync(); 
        }
        finally
        {
            _isSaving = false;
        }
    }

    private string GetRowClass(OpeningSlot slot)
    {
        if (slot.Status == SlotStatus.Aberto) return "table-success"; 
        if (slot.Status == SlotStatus.Cancelado) return "table-secondary text-muted text-decoration-line-through";
        if (slot.IsExtra && slot.Status == SlotStatus.Planejado) return "table-warning"; 
        return "";
    }
}