using OSSAcademicService.Domain.Entities;

namespace OSSAcademicService.Domain.Services;

/// <summary>
/// 选课领域服务
/// </summary>
public class SelectionDomainService
{
    public bool CanSelectCourse(StudentProfile student, SelectionRound round)
    {
        if (!student.IsEligibleForSelection())
            return false;

        if (!round.IsActive())
            return false;

        return true;
    }

    public bool HasScheduleConflict(List<ScheduleItem> newItems, List<ScheduleItem> existingItems)
    {
        foreach (var newItem in newItems)
        {
            foreach (var existing in existingItems)
            {
                if (newItem.OverlapsWith(existing))
                    return true;
            }
        }
        return false;
    }
}