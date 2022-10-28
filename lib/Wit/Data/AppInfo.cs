using System;

namespace Wit.Data
{
    public record AppInfo(
        string Id,
        string Name,
        string Lang,
        bool Private,
        DateTime CreatedAt,
        bool? IsAppForToken,
        int? LastTrainingDurationSecs,
        DateTime? WillTrainAt,
        DateTime? LastTrainedAt,
        string TrainingStatus
    );
}