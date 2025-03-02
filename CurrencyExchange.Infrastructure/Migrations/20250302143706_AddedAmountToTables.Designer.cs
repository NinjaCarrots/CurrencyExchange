﻿// <auto-generated />
using System;
using CurrencyExchange.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CurrencyExchange.Infrastructure.Migrations
{
    [DbContext(typeof(CurrencyExchangeDbContext))]
    [Migration("20250302143706_AddedAmountToTables")]
    partial class AddedAmountToTables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("CurrencyExchange.Domain.Entities.CurrencyExchangeHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("BaseCurrency")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<decimal>("ConvertedAmount")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("ExchangeRate")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("TargetCurrency")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("currencyExchangeHistories");
                });

            modelBuilder.Entity("CurrencyExchange.Domain.Entities.CurrencyRate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("BaseCurrency")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<decimal>("ExchangeRate")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("TargetCurrency")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("currencyRates");
                });
#pragma warning restore 612, 618
        }
    }
}
