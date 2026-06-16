using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PMS.Entities;

namespace PMS.Data;

public class PmsDbContext(DbContextOptions<PmsDbContext> options) : DbContext(options)
{
    private static readonly ValueConverter<string, string> EncryptedStringConverter = new(
        value => StaticDataEncryption.Encrypt(value),
        value => StaticDataEncryption.Decrypt(value));

    private static readonly ValueConverter<DateTime, string> EncryptedDateTimeConverter = new(
        value => StaticDataEncryption.Encrypt(value.ToString("O", CultureInfo.InvariantCulture)),
        value => DateTime.Parse(StaticDataEncryption.Decrypt(value), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind));

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public DbSet<Participant> Participants => Set<Participant>();

    public DbSet<Question> Questions => Set<Question>();

    public DbSet<Questionnaire> Questionnaires => Set<Questionnaire>();

    public DbSet<QuestionnaireAssignment> QuestionnaireAssignments => Set<QuestionnaireAssignment>();

    public DbSet<SelectableQuestionChoice> SelectableQuestionChoices => Set<SelectableQuestionChoice>();

    public DbSet<QuestionResponse> QuestionResponses => Set<QuestionResponse>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Questionnaire>(entity =>
        {
            entity.Property(questionnaire => questionnaire.Title)
                .HasMaxLength(200)
                .IsRequired();
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.Property(question => question.Text)
                .HasMaxLength(1000)
                .IsRequired();

            entity.HasOne(question => question.Questionnaire)
                .WithMany(questionnaire => questionnaire.Questions)
                .HasForeignKey(question => question.QuestionnaireId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(question => question.ParentQuestion)
                .WithMany()
                .HasForeignKey(question => question.ParentQuestionId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(question => question.ParentChoice)
                .WithMany()
                .HasForeignKey(question => question.ParentChoiceId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<SelectableQuestionChoice>(entity =>
        {
            entity.Property(choice => choice.ChoiceText)
                .HasMaxLength(500)
                .IsRequired();

            entity.HasOne(choice => choice.Question)
                .WithMany(question => question.Choices)
                .HasForeignKey(choice => choice.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Participant>(entity =>
        {
            entity.Property(participant => participant.FirstName)
                .HasConversion(EncryptedStringConverter)
                .HasMaxLength(1000)
                .IsRequired();

            entity.Property(participant => participant.LastName)
                .HasConversion(EncryptedStringConverter)
                .HasMaxLength(1000)
                .IsRequired();

            entity.Property(participant => participant.EmailAddress)
                .HasConversion(EncryptedStringConverter)
                .HasMaxLength(1000)
                .IsRequired();

            entity.Property(participant => participant.DateOfBirth)
                .HasConversion(EncryptedDateTimeConverter)
                .HasMaxLength(1000)
                .IsRequired();

            entity.Property(participant => participant.ParticipantCode)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(participant => participant.ParticipantCode)
                .IsUnique();

            entity.HasIndex(participant => participant.EmailAddress)
                .IsUnique();
        });

        modelBuilder.Entity<QuestionnaireAssignment>(entity =>
        {
            entity.HasOne(assignment => assignment.Participant)
                .WithMany()
                .HasForeignKey(assignment => assignment.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(assignment => assignment.Questionnaire)
                .WithMany()
                .HasForeignKey(assignment => assignment.QuestionnaireId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(assignment => new { assignment.ParticipantId, assignment.QuestionnaireId });
        });

        modelBuilder.Entity<QuestionResponse>(entity =>
        {
            entity.Property(response => response.TextValue)
                .HasMaxLength(4000);

            entity.Property(response => response.NumericValue)
                .HasPrecision(18, 2);

            entity.Property(response => response.SelectedChoiceIds)
                .HasMaxLength(1000);

            entity.HasOne(response => response.QuestionnaireAssignment)
                .WithMany()
                .HasForeignKey(response => response.QuestionnaireAssignmentId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(response => response.Question)
                .WithMany()
                .HasForeignKey(response => response.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.Property(auditLog => auditLog.EntityName)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(auditLog => auditLog.Action)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(auditLog => auditLog.Details)
                .HasMaxLength(4000);
        });
    }
}
