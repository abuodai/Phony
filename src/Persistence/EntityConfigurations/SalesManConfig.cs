﻿using System.Data.Entity.ModelConfiguration;

namespace Phony.Persistence.EntityConfigurations
{
    public class SalesManConfig : EntityTypeConfiguration<Model.SalesMan>
    {
        public SalesManConfig()
        {
            HasKey(c => c.Id);

            Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(50);

            HasIndex(c => c.Name)
                    .IsUnique();

            HasRequired(c => c.Creator)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .WillCascadeOnDelete(false);

            Property(c => c.EditById)
                .IsOptional();

            HasOptional(c => c.Editor)
                .WithMany()
                .HasForeignKey(s => s.EditById)
                .WillCascadeOnDelete(false);
        }
    }
}