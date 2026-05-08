using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LangSpace.Models;

public partial class LangSpaceContext : DbContext
{
    public LangSpaceContext()
    {
    }

    public LangSpaceContext(DbContextOptions<LangSpaceContext> options)
        : base(options)
    {
    }


    public virtual DbSet<Card> Cards { get; set; }
    public virtual DbSet<Goal> Goals { get; set; }
    public virtual DbSet<GrammarRule> GrammarRules { get; set; }
    public virtual DbSet<Language> Languages { get; set; }
    public virtual DbSet<Level> Levels { get; set; }
    public virtual DbSet<Note> Notes { get; set; }

    public virtual DbSet<Resource> Resources { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<TestQuestion> TestQuestions { get; set; }
    public virtual DbSet<TestResult> TestResults { get; set; }
    public virtual DbSet<Theme> Themes { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserLanguage> UserLanguages { get; set; }
    public virtual DbSet<GrammarQuestion> GrammarQuestions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Database=LangSpace;Username=postgres;Password=minion");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        

        modelBuilder.Entity<Card>(entity =>
        {
            entity.HasKey(e => e.IdCard).HasName("card_pkey");
            entity.ToTable("card");
            entity.Property(e => e.IdCard).HasColumnName("id_card");
            entity.Property(e => e.BackText).HasColumnName("back_text");
            entity.Property(e => e.FrontText).HasColumnName("front_text");
            entity.Property(e => e.IdLanguage).HasColumnName("id_language");
            entity.Property(e => e.IdLevel).HasColumnName("id_level");
            entity.Property(e => e.IdTheme).HasColumnName("id_theme");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.IsPrivate)
                .HasDefaultValue(false)
                .HasColumnName("is_private");

            entity.HasOne(d => d.IdLanguageNavigation).WithMany(p => p.Cards)
                .HasForeignKey(d => d.IdLanguage)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_card_language");

            entity.HasOne(d => d.IdLevelNavigation).WithMany(p => p.Cards)
                .HasForeignKey(d => d.IdLevel)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("card_id_level_fkey");

            entity.HasOne(d => d.IdThemeNavigation).WithMany(p => p.Cards)
                .HasForeignKey(d => d.IdTheme)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("card_id_theme_fkey");

            entity.HasOne(d => d.IdUserNavigation)
                .WithMany()
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_card_user");
        });
        
      

        modelBuilder.Entity<Goal>(entity =>
        {
            entity.HasKey(e => e.IdGoal).HasName("goal_pkey");
            entity.ToTable("goal");
            entity.Property(e => e.IdGoal).HasColumnName("id_goal");
   
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.CardsPerDay).HasColumnName("cards_per_day");
            entity.Property(e => e.TestsPerDay).HasColumnName("tests_per_day");
            entity.Property(e => e.DaysStreak).HasColumnName("days_streak");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Goals)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("goal_id_user_fkey");
        });

        modelBuilder.Entity<GrammarRule>(entity =>
        {
            entity.HasKey(e => e.IdRule).HasName("grammar_rule_pkey");
            entity.ToTable("grammar_rule");
            entity.Property(e => e.IdRule).HasColumnName("id_rule");
            entity.Property(e => e.IdLanguage).HasColumnName("id_language");
            entity.Property(e => e.IdLevel).HasColumnName("id_level");
            entity.Property(e => e.RuleName)
                .HasMaxLength(150)
                .HasColumnName("rule_name");
            entity.Property(e => e.RuleText).HasColumnName("rule_text");

            entity.HasOne(d => d.IdLanguageNavigation).WithMany(p => p.GrammarRules)
                .HasForeignKey(d => d.IdLanguage)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_grammar_rule_language");

            entity.HasOne(d => d.IdLevelNavigation).WithMany(p => p.GrammarRules)
                .HasForeignKey(d => d.IdLevel)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("grammar_rule_id_level_fkey");
        });

       

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.IdLanguage).HasName("languages_pkey");
            entity.ToTable("languages");
            entity.HasIndex(e => e.LanguageName, "languages_language_name_key").IsUnique();
            entity.Property(e => e.IdLanguage).HasColumnName("id_language");
            entity.Property(e => e.LanguageName)
                .HasMaxLength(100)
                .HasColumnName("language_name");
        });

        modelBuilder.Entity<Level>(entity =>
        {
            entity.HasKey(e => e.IdLevel).HasName("levels_pkey");
            entity.ToTable("levels");
            entity.HasIndex(e => e.LevelName, "levels_level_name_key").IsUnique();
            entity.Property(e => e.IdLevel).HasColumnName("id_level");
            entity.Property(e => e.LevelName)
                .HasMaxLength(10)
                .HasColumnName("level_name");
        });

        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.IdNote).HasName("note_pkey");
            entity.ToTable("note");
            entity.Property(e => e.IdNote).HasColumnName("id_note");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("date_created");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.NoteText).HasColumnName("note_text");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Notes)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("note_id_user_fkey");
        });

      

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(e => e.IdResource).HasName("resource_pkey");
            entity.ToTable("resource");
            entity.Property(e => e.IdResource).HasColumnName("id_resource");
            entity.Property(e => e.IdNote).HasColumnName("id_note");
            entity.Property(e => e.ResourceLink).HasColumnName("resource_link");
            entity.Property(e => e.ResourceType)
                .HasMaxLength(50)
                .HasColumnName("resource_type");
            entity.Property(e => e.ResourceText).HasColumnName("resource_text");

            entity.HasOne(d => d.IdNoteNavigation).WithMany(p => p.Resources)
                .HasForeignKey(d => d.IdNote)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("resource_id_note_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole).HasName("roles_pkey");
            entity.ToTable("roles");
            entity.HasIndex(e => e.RoleName, "roles_role_name_key").IsUnique();
            entity.Property(e => e.IdRole).HasColumnName("id_role");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<TestQuestion>(entity =>
        {
            entity.HasKey(e => e.IdQuestion).HasName("test_question_pkey");
            entity.ToTable("test_question");
            entity.Property(e => e.IdQuestion).HasColumnName("id_question");
            entity.Property(e => e.CorrectOption)
                .HasMaxLength(1)
                .HasColumnName("correct_option");
            entity.Property(e => e.IdLevel).HasColumnName("id_level");
            entity.Property(e => e.OptionA).HasColumnName("option_a");
            entity.Property(e => e.OptionB).HasColumnName("option_b");
            entity.Property(e => e.OptionC).HasColumnName("option_c");
            entity.Property(e => e.OptionD).HasColumnName("option_d");
            entity.Property(e => e.QuestionText).HasColumnName("question_text");
            entity.Property(e => e.QuestionType)
                .HasMaxLength(10)
                .HasColumnName("question_type");

            entity.HasOne(d => d.IdLevelNavigation).WithMany(p => p.TestQuestions)
                .HasForeignKey(d => d.IdLevel)
                .HasConstraintName("test_question_id_level_fkey");
        });

        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.HasKey(e => e.IdTestResult).HasName("test_result_pkey");
            entity.ToTable("test_result");
            entity.Property(e => e.IdTestResult).HasColumnName("id_test_result");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.TestType)
                .HasMaxLength(20)
                .HasColumnName("test_type");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.Total).HasColumnName("total");
            entity.Property(e => e.Date)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date");

            entity.HasOne(d => d.IdUserNavigation)
                .WithMany(p => p.TestResults)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("fk_test_result_user");
        });

        modelBuilder.Entity<Theme>(entity =>
        {
            entity.HasKey(e => e.IdTheme).HasName("theme_pkey");
            entity.ToTable("theme");
            entity.HasIndex(e => e.ThemeName, "theme_theme_name_key").IsUnique();
            entity.Property(e => e.IdTheme).HasColumnName("id_theme");
            entity.Property(e => e.ThemeName)
                .HasMaxLength(100)
                .HasColumnName("theme_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("users_pkey");
            entity.ToTable("users");
            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();
            entity.HasIndex(e => e.Username, "users_username_key").IsUnique();
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("registration_date");
            entity.Property(e => e.IdRole).HasColumnName("id_role");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .HasColumnName("avatar");

            entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_id_role_fkey");
        });

        modelBuilder.Entity<UserLanguage>(entity =>
        {
            entity.HasKey(e => new { e.IdUser, e.IdLanguage }).HasName("user_language_pkey");
            entity.ToTable("user_language");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.IdLanguage).HasColumnName("id_language");
            entity.Property(e => e.IdLevel).HasColumnName("id_level");

            entity.HasOne(d => d.IdLanguageNavigation).WithMany(p => p.UserLanguages)
                .HasForeignKey(d => d.IdLanguage)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_language_id_language_fkey");

            entity.HasOne(d => d.IdLevelNavigation).WithMany(p => p.UserLanguages)
                .HasForeignKey(d => d.IdLevel)
                .HasConstraintName("fk_user_language_level");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserLanguages)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_language_id_user_fkey");
        });

        modelBuilder.Entity<GrammarQuestion>(entity =>
        {
            entity.HasKey(e => e.IdGrammarQuestion).HasName("grammar_question_pkey");
            entity.ToTable("grammar_question");
            entity.Property(e => e.IdGrammarQuestion).HasColumnName("id_grammar_question");
            entity.Property(e => e.QuestionText).HasColumnName("question_text");
            entity.Property(e => e.Option1).HasColumnName("option1");
            entity.Property(e => e.Option2).HasColumnName("option2");
            entity.Property(e => e.Option3).HasColumnName("option3");
            entity.Property(e => e.Option4).HasColumnName("option4");
            entity.Property(e => e.CorrectAnswer).HasColumnName("correct_answer");
            entity.Property(e => e.IdGrammarRule).HasColumnName("id_grammar_rule");

            entity.HasOne(d => d.IdGrammarRuleNavigation)
                .WithMany(p => p.GrammarQuestions)
                .HasForeignKey(d => d.IdGrammarRule)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_grammar_question_rule");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}