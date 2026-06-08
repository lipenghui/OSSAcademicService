namespace OSSAcademicService.Domain.Enums;

/// <summary>
/// 学籍状态
/// </summary>
public enum StudentStatus
{
    /// <summary>在读</summary>
    Enrolled = 1,
    /// <summary>休学</summary>
    Suspended = 2,
    /// <summary>退学</summary>
    Withdrawn = 3,
    /// <summary>毕业</summary>
    Graduated = 4,
    /// <summary>结业</summary>
    Finished = 5,
    /// <summary>肄业</summary>
    Incomplete = 6,
    /// <summary>试读</summary>
    Probation = 7
}

/// <summary>
/// 学籍异动类型
/// </summary>
public enum ChangeType
{
    /// <summary>转专业</summary>
    TransferMajor = 1,
    /// <summary>转学</summary>
    TransferSchool = 2,
    /// <summary>休学</summary>
    Suspension = 3,
    /// <summary>复学</summary>
    Resumption = 4,
    /// <summary>退学</summary>
    Withdrawal = 5,
    /// <summary>留级</summary>
    Retention = 6,
    /// <summary>延长学制</summary>
    Extension = 7,
    /// <summary>毕业</summary>
    Graduation = 8,
    /// <summary>其他</summary>
    Other = 9
}

/// <summary>
/// 课程分类
/// </summary>
public enum CourseType
{
    /// <summary>公共必修</summary>
    PublicCompulsory = 1,
    /// <summary>专业必修</summary>
    MajorCompulsory = 2,
    /// <summary>专业选修</summary>
    MajorElective = 3,
    /// <summary>通识选修</summary>
    GeneralElective = 4
}

/// <summary>
/// 授课方式
/// </summary>
public enum TeachingMode
{
    /// <summary>讲授</summary>
    Lecture = 1,
    /// <summary>实验</summary>
    Lab = 2,
    /// <summary>实践</summary>
    Practice = 3,
    /// <summary>混合</summary>
    Mixed = 4
}

/// <summary>
/// 成绩类型
/// </summary>
public enum ScoreType
{
    /// <summary>百分制</summary>
    Percentage = 1,
    /// <summary>五级制</summary>
    FiveGrade = 2,
    /// <summary>二级制</summary>
    TwoGrade = 3
}

/// <summary>
/// 考试类型
/// </summary>
public enum ExamType
{
    /// <summary>期末</summary>
    Final = 1,
    /// <summary>期中</summary>
    Midterm = 2,
    /// <summary>补考</summary>
    Makeup = 3,
    /// <summary>重修</summary>
    Retake = 4
}

/// <summary>
/// 周次类型
/// </summary>
public enum WeekType
{
    /// <summary>每周</summary>
    Every = 1,
    /// <summary>单周</summary>
    Odd = 2,
    /// <summary>双周</summary>
    Even = 3
}

/// <summary>
/// 教室类型
/// </summary>
public enum RoomType
{
    /// <summary>普通教室</summary>
    Normal = 1,
    /// <summary>实验室</summary>
    Lab = 2,
    /// <summary>机房</summary>
    ComputerRoom = 3,
    /// <summary>多媒体教室</summary>
    Multimedia = 4,
    /// <summary>阶梯教室</summary>
    LectureHall = 5
}

/// <summary>
/// 选课轮次类型
/// </summary>
public enum SelectionRoundType
{
    /// <summary>预选</summary>
    PreSelection = 1,
    /// <summary>正选</summary>
    MainSelection = 2,
    /// <summary>补退选</summary>
    AddDrop = 3
}

/// <summary>
/// 审批状态
/// </summary>
public enum ApplyStatus
{
    /// <summary>待提交</summary>
    Pending = 0,
    /// <summary>辅导员审核中</summary>
    CounselorReviewing = 1,
    /// <summary>院系审批中</summary>
    DepartmentApproving = 2,
    /// <summary>教务处备案中</summary>
    RegistrarFiling = 3,
    /// <summary>已通过</summary>
    Approved = 4,
    /// <summary>已驳回</summary>
    Rejected = 5
}

/// <summary>
/// 毕业审核结果
/// </summary>
public enum GraduationResult
{
    /// <summary>待审核</summary>
    Pending = 0,
    /// <summary>通过</summary>
    Passed = 1,
    /// <summary>不通过</summary>
    Failed = 2,
    /// <summary>结业</summary>
    Finished = 3,
    /// <summary>肄业</summary>
    Incomplete = 4
}

/// <summary>
/// 调课类型
/// </summary>
public enum AdjustType
{
    /// <summary>调课</summary>
    Adjust = 1,
    /// <summary>停课</summary>
    Stop = 2,
    /// <summary>补课</summary>
    Supplement = 3
}