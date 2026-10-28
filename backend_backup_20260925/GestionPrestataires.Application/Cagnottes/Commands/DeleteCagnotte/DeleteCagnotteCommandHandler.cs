using GestionPrestataires.Application.Cagnottes.Commands.DeleteSourceBudget;
using GestionPrestataires.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Application.Cagnottes.Commands.DeleteCagnotte;
public class DeleteCagnotteCommandHandler : IRequestHandler<DeleteCagnotteCommand, Unit>
{
    private readonly ISourceBudgetRepository _sourceBudgetRepository;
    private readonly ICagnotteRepository _cagnotteRepository;

    public DeleteCagnotteCommandHandler(
        ISourceBudgetRepository sourceBudgetRepository,
        ICagnotteRepository cagnotteRepository)
    {
        _sourceBudgetRepository = sourceBudgetRepository;
        _cagnotteRepository = cagnotteRepository;
    }

    public async Task<Unit> Handle(DeleteCagnotteCommand request, CancellationToken cancellationToken)
    {
        var cagnotte = await _cagnotteRepository.GetByIdAsync(request.Id);
        if (cagnotte == null)
            throw new Exception("Cagnotte non trouvée");
        //if (cagnotte.SourcesBudget != null)
        //{
        //    foreach (var item in cagnotte.SourcesBudget)
        //    {
        //        try
        //        {
        //            await _sourceBudgetRepository.DeleteAsync(item.Id);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("Cagnotte non trouvée : " + ex.Message);

        //        }
        //    }
        //}
        await _cagnotteRepository.DeleteAsync(cagnotte.Id);

        return Unit.Value;
    }
}