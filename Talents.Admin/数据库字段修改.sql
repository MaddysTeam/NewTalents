
/*
�걨-����
*/
select * from DeclareContent

select * from [NewTalents20161115].[dbo].DeclareContent

	update DeclareContent
	set Creator = TeacherId

	update DeclareContent
	set CreateDate = convert(varchar(20), '2016-11-11', 120)
	where DeclareContentId in (select DeclareContentId from [NewTalents20161115].[dbo].DeclareContent)
  
	update DeclareContent
	set CreateDate = getDate()
	where DeclareContentId not in (select DeclareContentId from [NewTalents20161115].[dbo].DeclareContent)

/*
�걨-�
*/
select * from DeclareActive

select * from [NewTalents20161115].[dbo].DeclareActive

	update DeclareActive
	set Creator = TeacherId

	update DeclareActive
	set CreateDate = convert(varchar(20), '2016-11-11', 120)
	where DeclareActiveId in (select DeclareActiveId from [NewTalents20161115].[dbo].DeclareActive)

	update DeclareActive
	set CreateDate = getdate()
	where DeclareActiveId not in (select DeclareActiveId from [NewTalents20161115].[dbo].DeclareActive)
	
/*
�걨-���гɹ� (DeclareAchievement)
*/

select * from DeclareAchievement

select * from [NewTalents20161115].[dbo].DeclareAchievement
	
	update DeclareAchievement
	set Creator = TeacherId
	
	update DeclareAchievement
	set CreateDate = convert(varchar(20), '2016-11-11', 120)
	where DeclareAchievementId in (select DeclareAchievementId from [NewTalents20161115].[dbo].DeclareAchievement)	
	
	update DeclareAchievement
	set CreateDate = getdate()
	where DeclareAchievementId not in (select DeclareAchievementId from [NewTalents20161115].[dbo].DeclareAchievement)
	
/*
�걨-�ƶȽ��� (DeclareOrgConst)
*/

select * from DeclareOrgConst

select * from [NewTalents20161115].[dbo].DeclareOrgConst

	update DeclareOrgConst
	set Creator = TeacherId

	update DeclareOrgConst
	set CreateDate = convert(varchar(20), '2016-11-11', 120)
	where DeclareOrgConstId in  (select DeclareOrgConstId from [NewTalents20161115].[dbo].DeclareOrgConst)
	
	update DeclareOrgConst
	set CreateDate = getdate()
	where DeclareOrgConstId not in  (select DeclareOrgConstId from [NewTalents20161115].[dbo].DeclareOrgConst)
	
/*
�걨-���� (DeclareResume)
*/

select * from DeclareResume

select * from [NewTalents20161115].[dbo].DeclareResume

	update DeclareResume
	set Creator = TeacherId
	
	update DeclareResume
	set CreateDate = convert(varchar(20),'2016-11-11',120)
	where DeclareResumeId in (select DeclareResumeId from [NewTalents20161115].[dbo].DeclareResume)	
	
	update DeclareResume
	set CreateDate = getdate()
	where DeclareResumeId not in (select DeclareResumeId from [NewTalents20161115].[dbo].DeclareResume)
	
/*
�ݶ�-ѧԱ (TeamMember)
*/

select * from TeamMember

select * from [NewTalents20161115].[dbo].TeamMember


/*
����������Ϊ����Ա�� ��ǰ���ݿ��˺Ź���ԱidΪ1
*/
update TeamMember
set creator = 1

update TeamMember
set createDate = getdate()
from teamMember t, [NewTalents20161115].[dbo].TeamMember t1
where not exists (select 1 from [NewTalents20161115].[dbo].TeamMember t1 where t.teamId = t1.teamId and t.memberId = t1.memberId)

update TeamMember
set createDate = convert(varchar(20), '2016-11-11', 120)
from teamMember t, [NewTalents20161115].[dbo].TeamMember t1
where exists (select 1 from [NewTalents20161115].[dbo].TeamMember t1 where t.teamId = t1.teamId and t.memberId = t1.memberId)

/*
�ݶ�-���� ��TeamContent��
*/
select * from TeamContent

select * from [NewTalents20161115].[dbo].TeamContent

	update TeamContent
	set creator = TeamId	

	update TeamContent
	set createDate = convert(varchar(20), '2016-11-11', 120)
	where TeamContentId in (select TeamContentId from [NewTalents20161115].[dbo].TeamContent)
	
	update TeamContent
	set createDate = getdate()
	where TeamContentId not in (select TeamContentId from [NewTalents20161115].[dbo].TeamContent)
	
/*
�ݶ�-� ��TeamActive��
*/
select * from TeamActive

select * from [NewTalents20161115].[dbo].TeamActive

	update TeamActive
	set creator = TeamId
	
	update TeamActive
	set createDate = convert(varchar(20), '2016-11-11', 120)
	where TeamActiveId in (select TeamActiveId from [NewTalents20161115].[dbo].TeamActive)
	
	update TeamActive
	set createDate = getdate()
	where TeamActiveId not in (select TeamActiveId from [NewTalents20161115].[dbo].TeamActive)
	
/*
�ݶ�-��ɹ�����TeamActiveResult��
*/	
select * from TeamActiveResult

select * from [NewTalents20161115].[dbo].TeamActiveResult

	update TeamActiveResult
	set Creator = MemberId
	
	update TeamActiveResult
	set createDate = convert(varchar(20), '2016-11-11', 120)
	where resultId in (select resultId from [NewTalents20161115].[dbo].TeamActiveResult)
	
	update TeamActiveResult
	set createDate = getdate()
	where resultId not in (select resultId from [NewTalents20161115].[dbo].TeamActiveResult)	

/*
�ݶ�-����� (TeamActiveItem)
*/
select * from TeamActiveItem

select * from [NewTalents20161115].[dbo].TeamActiveItem

	update TeamActiveItem
	set creator = MemberId
	
	update TeamActiveItem
	set createdate = convert(varchar(20), '2016-11-11', 120)
	where ItemId in (select ItemId from [NewTalents20161115].[dbo].TeamActiveItem)
	
	update TeamActiveItem
	set createdate = getdate()
	where ItemId not in (select ItemId from [NewTalents20161115].[dbo].TeamActiveItem)
	
/*
�ݶ�-�����Կγ̿���ǼǱ� (TeamSpecialCourse)
*/
select * from TeamSpecialCourse

select * from [NewTalents20161115].[dbo].TeamSpecialCourse

	update 	TeamSpecialCourse
	set creator = TeamId
	
	update TeamSpecialCourse
	set createdate = convert(varchar(20), '2016-11-11', 120)
	where CourseId in (select CourseId from [NewTalents20161115].[dbo].TeamSpecialCourse)
	
	update TeamSpecialCourse
	set createdate = getdate()
	where CourseId not in (select CourseId from [NewTalents20161115].[dbo].TeamSpecialCourse)
	
/*
�ݶ�-�����Կγ�ʵʩ���ű�(TeamSpecialCourseItem)
*/

select * from TeamSpecialCourseItem

select * from [NewTalents20161115].[dbo].TeamSpecialCourseItem

	update TeamSpecialCourseItem
	set creator = TeamId
	
	update TeamSpecialCourseItem
	set createdate = convert(varchar(20), '2016-11-11', 120)
	where ItemId in (select ItemId from [NewTalents20161115].[dbo].TeamSpecialCourseItem)
	
	update TeamSpecialCourseItem
	set createdate = getdate()
	where ItemId not in (select ItemId from [NewTalents20161115].[dbo].TeamSpecialCourseItem)
	
/*
ר���� (ExpGroup)
*/
select * from ExpGroup

select * from [NewTalents20161115].[dbo].ExpGroup

	update ExpGroup
	set creator = 1
	
	update ExpGroup
	set createdate = convert(varchar(20), '2016-11-11', 120)
	where groupid in (select groupid from [NewTalents20161115].[dbo].ExpGroup)
	
	update ExpGroup
	set createdate = getdate()
	where groupid not in (select groupid from [NewTalents20161115].[dbo].ExpGroup) 
	
/*
����-���� (EvalPeriod)
*/
select * from EvalPeriod

select * from [NewTalents20161115].[dbo].EvalPeriod

	update EvalPeriod
	set creator = 1
	
	update EvalPeriod
	set createdate = convert(varchar(20), '2016-11-11', 120)
	where PeriodId in (select PeriodId from [NewTalents20161115].[dbo].EvalPeriod)
	
	update EvalPeriod
	set createdate = getdate()
	where PeriodId not in (select PeriodId from [NewTalents20161115].[dbo].EvalPeriod)