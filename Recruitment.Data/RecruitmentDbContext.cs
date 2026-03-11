using Microsoft.EntityFrameworkCore;
using Recruitment.Entities;

namespace Recruitment.Data
{
    public class RecruitmentDbContext : DbContext
    {
        public RecruitmentDbContext(DbContextOptions<RecruitmentDbContext> options) : base(options)
        {
        }

        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Recruiter> Recruiters { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<JobSkill> JobSkills { get; set; }
        public DbSet<CandidateSkill> CandidateSkills { get; set; }
        public DbSet<JobMatch> JobMatches { get; set; }
        public DbSet<InterviewQuestion> InterviewQuestions { get; set; }
        public DbSet<CVAnalysis> CVAnalyses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Candidate relaties
            modelBuilder.Entity<Candidate>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(c => c.LastName).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Email).IsRequired().HasMaxLength(200);
                entity.Property(c => c.Phone).HasMaxLength(50);
                entity.Property(c => c.Location).HasMaxLength(200);
                entity.Property(c => c.CVFilePath).HasMaxLength(500);
                entity.Property(c => c.CreatedAt).IsRequired();

                entity.HasMany(c => c.CandidateSkills)
                    .WithOne(cs => cs.Candidate)
                    .HasForeignKey(cs => cs.CandidateId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.JobMatches)
                    .WithOne(jm => jm.Candidate)
                    .HasForeignKey(jm => jm.CandidateId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.InterviewQuestions)
                    .WithOne(iq => iq.Candidate)
                    .HasForeignKey(iq => iq.CandidateId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.CVAnalysis)
                    .WithOne(cv => cv.Candidate)
                    .HasForeignKey<CVAnalysis>(cv => cv.CandidateId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Recruiter relaties
            modelBuilder.Entity<Recruiter>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(r => r.LastName).IsRequired().HasMaxLength(100);
                entity.Property(r => r.Email).IsRequired().HasMaxLength(200);
                entity.Property(r => r.Company).HasMaxLength(200);
                entity.Property(r => r.CreatedAt).IsRequired();

                entity.HasMany(r => r.Jobs)
                    .WithOne(j => j.Recruiter)
                    .HasForeignKey(j => j.RecruiterId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Job relaties
            modelBuilder.Entity<Job>(entity =>
            {
                entity.HasKey(j => j.Id);
                entity.Property(j => j.Title).IsRequired().HasMaxLength(200);
                entity.Property(j => j.Description).HasMaxLength(2000);
                entity.Property(j => j.Location).HasMaxLength(200);
                entity.Property(j => j.ExperienceLevel).HasMaxLength(100);
                entity.Property(j => j.EducationLevel).HasMaxLength(100);
                entity.Property(j => j.CreatedAt).IsRequired();

                entity.HasMany(j => j.JobSkills)
                    .WithOne(js => js.Job)
                    .HasForeignKey(js => js.JobId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(j => j.JobMatches)
                    .WithOne(jm => jm.Job)
                    .HasForeignKey(jm => jm.JobId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(j => j.InterviewQuestions)
                    .WithOne(iq => iq.Job)
                    .HasForeignKey(iq => iq.JobId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Skill relaties
            modelBuilder.Entity<Skill>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.SkillName).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Category).HasMaxLength(100);

                entity.HasMany(s => s.JobSkills)
                    .WithOne(js => js.Skill)
                    .HasForeignKey(js => js.SkillId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(s => s.CandidateSkills)
                    .WithOne(cs => cs.Skill)
                    .HasForeignKey(cs => cs.SkillId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // JobSkill relaties
            modelBuilder.Entity<JobSkill>(entity =>
            {
                entity.HasKey(js => js.Id);
                entity.Property(js => js.RequiredLevel).IsRequired();
                entity.Property(js => js.Weight).IsRequired();
            });

            // CandidateSkill relaties
            modelBuilder.Entity<CandidateSkill>(entity =>
            {
                entity.HasKey(cs => cs.Id);
                entity.Property(cs => cs.Level).IsRequired();
            });

            // JobMatch relaties
            modelBuilder.Entity<JobMatch>(entity =>
            {
                entity.HasKey(jm => jm.Id);
                entity.Property(jm => jm.MatchScore).IsRequired();
                entity.Property(jm => jm.SkillGap).HasMaxLength(1000);
                entity.Property(jm => jm.CreatedAt).IsRequired();
            });

            // InterviewQuestion relaties
            modelBuilder.Entity<InterviewQuestion>(entity =>
            {
                entity.HasKey(iq => iq.Id);
                entity.Property(iq => iq.Question).IsRequired().HasMaxLength(1000);
                entity.Property(iq => iq.CreatedAt).IsRequired();
            });

            // CVAnalysis relaties
            modelBuilder.Entity<CVAnalysis>(entity =>
            {
                entity.HasKey(cv => cv.Id);
                entity.Property(cv => cv.Summary).HasMaxLength(2000);
                entity.Property(cv => cv.ExperienceLevel).HasMaxLength(100);
                entity.Property(cv => cv.Strengths).HasMaxLength(1000);
                entity.Property(cv => cv.Weaknesses).HasMaxLength(1000);
                entity.Property(cv => cv.CreatedAt).IsRequired();
            });
        }
    }
}
