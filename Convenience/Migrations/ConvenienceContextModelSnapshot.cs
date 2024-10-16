﻿// <auto-generated />
using System;
using Convenience.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Convenience.Migrations
{
    [DbContext(typeof(ConvenienceContext))]
    partial class ConvenienceContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Convenience.Models.DataModels.ChumonJisseki", b =>
                {
                    b.Property<string>("ChumonId")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("chumon_code");

                    b.Property<DateOnly>("ChumonDate")
                        .HasColumnType("date")
                        .HasColumnName("chumon_date");

                    b.Property<string>("ShiireSakiId")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_saki_code");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("ChumonId");

                    b.HasIndex("ShiireSakiId");

                    b.ToTable("chumon_jisseki");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.ChumonJissekiMeisai", b =>
                {
                    b.Property<string>("ChumonId")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("chumon_code");

                    b.Property<string>("ShiireSakiId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_saki_code");

                    b.Property<string>("ShiirePrdId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_prd_code");

                    b.Property<string>("ShohinId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shohin_code");

                    b.Property<decimal>("ChumonSu")
                        .HasPrecision(10, 2)
                        .HasColumnType("numeric(10,2)")
                        .HasColumnName("chumon_su");

                    b.Property<decimal>("ChumonZan")
                        .HasPrecision(10, 2)
                        .HasColumnType("numeric(10,2)")
                        .HasColumnName("chumon_zan");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("ChumonId", "ShiireSakiId", "ShiirePrdId", "ShohinId");

                    b.HasIndex("ShiireSakiId", "ShiirePrdId", "ShohinId");

                    b.ToTable("chumon_jisseki_meisai");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.KaikeiHeader", b =>
                {
                    b.Property<string>("UriageDatetimeId")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("uriage_datetimeid");

                    b.Property<DateTime>("UriageDatetime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("uriage_datetime");

                    b.HasKey("UriageDatetimeId");

                    b.ToTable("kaikei_header");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.KaikeiJisseki", b =>
                {
                    b.Property<string>("UriageDatetimeId")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("uriage_datetimeid");

                    b.Property<string>("ShohinId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shohin_code");

                    b.Property<DateTime>("UriageDatetime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("uriage_datetime");

                    b.Property<int>("KaikeiSeq")
                        .HasColumnType("integer")
                        .HasColumnName("kaikei_seq");

                    b.Property<string>("NaigaiClass")
                        .IsRequired()
                        .HasMaxLength(1)
                        .HasColumnType("character varying(1)")
                        .HasColumnName("naigai_class");

                    b.Property<decimal>("ShohiZeiritsu")
                        .HasPrecision(15, 2)
                        .HasColumnType("numeric(15,2)")
                        .HasColumnName("shohi_zeiritsu");

                    b.Property<decimal>("ShohinTanka")
                        .HasPrecision(15, 2)
                        .HasColumnType("numeric(15,2)")
                        .HasColumnName("shohin_tanka");

                    b.Property<decimal>("UriageKingaku")
                        .HasPrecision(15, 2)
                        .HasColumnType("numeric(15,2)")
                        .HasColumnName("uriage_kingaku");

                    b.Property<decimal>("UriageSu")
                        .HasPrecision(10, 2)
                        .HasColumnType("numeric(10,2)")
                        .HasColumnName("uriage_su");

                    b.Property<decimal>("ZeikomiKingaku")
                        .HasPrecision(15, 2)
                        .HasColumnType("numeric(15,2)")
                        .HasColumnName("zeikomi_kingaku");

                    b.HasKey("UriageDatetimeId", "ShohinId", "UriageDatetime");

                    b.HasIndex("NaigaiClass");

                    b.HasIndex("ShohinId");

                    b.ToTable("kaikei_jisseki");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.NaigaiClassMaster", b =>
                {
                    b.Property<string>("NaigaiClass")
                        .HasMaxLength(1)
                        .HasColumnType("character varying(1)")
                        .HasColumnName("naigai_class");

                    b.Property<string>("NaigaiClassName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("naigai_class_name");

                    b.HasKey("NaigaiClass");

                    b.ToTable("naigai_class_master");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.ShiireJisseki", b =>
                {
                    b.Property<string>("ChumonId")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("chumon_code");

                    b.Property<DateOnly>("ShiireDate")
                        .HasColumnType("date")
                        .HasColumnName("shiire_date");

                    b.Property<long>("SeqByShiireDate")
                        .HasColumnType("bigint")
                        .HasColumnName("seq_by_shiiredate");

                    b.Property<string>("ShiireSakiId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_saki_code");

                    b.Property<string>("ShiirePrdId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_prd_code");

                    b.Property<decimal>("NonyuSu")
                        .HasPrecision(10, 2)
                        .HasColumnType("numeric(10,2)")
                        .HasColumnName("nonyu_su");

                    b.Property<DateTime>("ShiireDateTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("shiire_datetime");

                    b.Property<string>("ShohinId")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shohin_code");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("ChumonId", "ShiireDate", "SeqByShiireDate", "ShiireSakiId", "ShiirePrdId");

                    b.HasIndex("ChumonId", "ShiireSakiId", "ShiirePrdId", "ShohinId");

                    b.ToTable("shiire_jisseki");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.ShiireMaster", b =>
                {
                    b.Property<string>("ShiireSakiId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_saki_code");

                    b.Property<string>("ShiirePrdId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_prd_code");

                    b.Property<string>("ShohinId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shohin_code");

                    b.Property<decimal>("ShiirePcsPerUnit")
                        .HasPrecision(7, 2)
                        .HasColumnType("numeric(7,2)")
                        .HasColumnName("shiire_pcs_unit");

                    b.Property<string>("ShiirePrdName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("shiire_prd_name");

                    b.Property<string>("ShiireUnit")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_unit");

                    b.Property<decimal>("ShireTanka")
                        .HasPrecision(7, 2)
                        .HasColumnType("numeric(7,2)")
                        .HasColumnName("shiire_tanka");

                    b.HasKey("ShiireSakiId", "ShiirePrdId", "ShohinId");

                    b.HasIndex("ShohinId");

                    b.ToTable("shiire_master");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.ShiireSakiMaster", b =>
                {
                    b.Property<string>("ShiireSakiId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_saki_code");

                    b.Property<string>("Banchi")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("banchi");

                    b.Property<string>("ShiireSakiBusho")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("shiire_saki_busho");

                    b.Property<string>("ShiireSakiKaisya")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("shiire_saki_kaisya");

                    b.Property<string>("Shikuchoson")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shikuchoson");

                    b.Property<string>("Tatemonomei")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("tatemonomei");

                    b.Property<string>("Todoufuken")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("todoufuken");

                    b.Property<string>("YubinBango")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("yubin_bango");

                    b.HasKey("ShiireSakiId");

                    b.ToTable("shiire_saki_master");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.ShohinMaster", b =>
                {
                    b.Property<string>("ShohinId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shohin_code");

                    b.Property<decimal>("ShohiZeiritsu")
                        .HasPrecision(15, 2)
                        .HasColumnType("numeric(15,2)")
                        .HasColumnName("shohi_zeiritsu");

                    b.Property<decimal>("ShohiZeiritsuEatIn")
                        .HasPrecision(15, 2)
                        .HasColumnType("numeric(15,2)")
                        .HasColumnName("shohi_zeiritsu_eatin");

                    b.Property<string>("ShohinName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("shohin_name");

                    b.Property<decimal>("ShohinTanka")
                        .HasPrecision(15, 2)
                        .HasColumnType("numeric(15,2)")
                        .HasColumnName("shohin_tanka");

                    b.HasKey("ShohinId");

                    b.ToTable("shohin_master");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.SokoZaiko", b =>
                {
                    b.Property<string>("ShiireSakiId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_saki_code");

                    b.Property<string>("ShiirePrdId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_prd_code");

                    b.Property<string>("ShohinId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shohin_code");

                    b.Property<DateOnly?>("LastDeliveryDate")
                        .HasColumnType("date")
                        .HasColumnName("last_delivery_date");

                    b.Property<DateOnly>("LastShiireDate")
                        .HasColumnType("date")
                        .HasColumnName("last_shiire_date");

                    b.Property<decimal>("SokoZaikoCaseSu")
                        .HasPrecision(10, 2)
                        .HasColumnType("numeric(10,2)")
                        .HasColumnName("soko_zaiko_case_su");

                    b.Property<decimal>("SokoZaikoSu")
                        .HasPrecision(10, 2)
                        .HasColumnType("numeric(10,2)")
                        .HasColumnName("soko_zaiko_su");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("ShiireSakiId", "ShiirePrdId", "ShohinId");

                    b.ToTable("soko_zaiko");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.TentoHaraidashiHeader", b =>
                {
                    b.Property<string>("TentoHaraidashiId")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("tento_haraidashi_code");

                    b.Property<DateTime>("HaraidashiDateTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("haraidashi_datetime");

                    b.HasKey("TentoHaraidashiId");

                    b.ToTable("tento_haraidashi_header");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.TentoHaraidashiJisseki", b =>
                {
                    b.Property<string>("TentoHaraidashiId")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("tento_haraidashi_code");

                    b.Property<string>("ShiireSakiId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_saki_code");

                    b.Property<string>("ShiirePrdId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_prd_code");

                    b.Property<string>("ShohinId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shohin_code");

                    b.Property<int>("HaraidashiCaseSu")
                        .HasColumnType("integer")
                        .HasColumnName("haraidashi_case_su");

                    b.Property<DateTime>("HaraidashiDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("haraidashi_date");

                    b.Property<decimal>("HaraidashiSu")
                        .HasPrecision(7, 2)
                        .HasColumnType("numeric(7,2)")
                        .HasColumnName("haraidashi_su");

                    b.Property<DateOnly>("ShireDateTime")
                        .HasColumnType("date")
                        .HasColumnName("shiire_datetime");

                    b.HasKey("TentoHaraidashiId", "ShiireSakiId", "ShiirePrdId", "ShohinId");

                    b.HasIndex("ShiireSakiId", "ShiirePrdId", "ShohinId");

                    b.ToTable("tento_haraidashi_jisseki");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.TentoZaiko", b =>
                {
                    b.Property<string>("ShohinId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shohin_code");

                    b.Property<DateTime?>("LastHaraidashiDate")
                        .IsRequired()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_haraidashi_datetime");

                    b.Property<DateOnly>("LastShireDateTime")
                        .HasColumnType("date")
                        .HasColumnName("last_shiire_datetime");

                    b.Property<DateTime?>("LastUriageDatetime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_uriage_datetime");

                    b.Property<decimal>("ZaikoSu")
                        .HasPrecision(7, 2)
                        .HasColumnType("numeric(7,2)")
                        .HasColumnName("zaiko_su");

                    b.HasKey("ShohinId");

                    b.ToTable("tento_zaiko");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.ChumonJisseki", b =>
                {
                    b.HasOne("Convenience.Models.DataModels.ShiireSakiMaster", "ShiireSakiMaster")
                        .WithMany("ChumonJissekis")
                        .HasForeignKey("ShiireSakiId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ShiireSakiMaster");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.ChumonJissekiMeisai", b =>
                {
                    b.HasOne("Convenience.Models.DataModels.ChumonJisseki", "ChumonJisseki")
                        .WithMany("ChumonJissekiMeisais")
                        .HasForeignKey("ChumonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Convenience.Models.DataModels.ShiireMaster", "ShiireMaster")
                        .WithMany("ChumonJissekiMeisaiis")
                        .HasForeignKey("ShiireSakiId", "ShiirePrdId", "ShohinId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ChumonJisseki");

                    b.Navigation("ShiireMaster");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.KaikeiJisseki", b =>
                {
                    b.HasOne("Convenience.Models.DataModels.NaigaiClassMaster", "NaigaiClassMaster")
                        .WithMany("KaikeiJissekis")
                        .HasForeignKey("NaigaiClass")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Convenience.Models.DataModels.ShohinMaster", "ShohinMaster")
                        .WithMany()
                        .HasForeignKey("ShohinId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Convenience.Models.DataModels.TentoZaiko", "TentoZaiko")
                        .WithMany()
                        .HasForeignKey("ShohinId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Convenience.Models.DataModels.KaikeiHeader", "KaikeiHeader")
                        .WithMany("KaikeiJissekis")
                        .HasForeignKey("UriageDatetimeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("KaikeiHeader");

                    b.Navigation("NaigaiClassMaster");

                    b.Navigation("ShohinMaster");

                    b.Navigation("TentoZaiko");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.ShiireJisseki", b =>
                {
                    b.HasOne("Convenience.Models.DataModels.ChumonJissekiMeisai", "ChumonJissekiMeisaii")
                        .WithMany("ShiireJisseki")
                        .HasForeignKey("ChumonId", "ShiireSakiId", "ShiirePrdId", "ShohinId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ChumonJissekiMeisaii");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.ShiireMaster", b =>
                {
                    b.HasOne("Convenience.Models.DataModels.ShiireSakiMaster", "ShiireSakiMaster")
                        .WithMany("ShireMasters")
                        .HasForeignKey("ShiireSakiId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Convenience.Models.DataModels.ShohinMaster", "ShohinMaster")
                        .WithMany("ShiireMasters")
                        .HasForeignKey("ShohinId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ShiireSakiMaster");

                    b.Navigation("ShohinMaster");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.SokoZaiko", b =>
                {
                    b.HasOne("Convenience.Models.DataModels.ShiireMaster", "ShiireMaster")
                        .WithOne("SokoZaiko")
                        .HasForeignKey("Convenience.Models.DataModels.SokoZaiko", "ShiireSakiId", "ShiirePrdId", "ShohinId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ShiireMaster");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.TentoHaraidashiJisseki", b =>
                {
                    b.HasOne("Convenience.Models.DataModels.TentoHaraidashiHeader", "TentoHaraidashiHeader")
                        .WithMany("TentoHaraidashiJissekis")
                        .HasForeignKey("TentoHaraidashiId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Convenience.Models.DataModels.ShiireMaster", "ShiireMaster")
                        .WithMany("TentoHaraidashiJissekis")
                        .HasForeignKey("ShiireSakiId", "ShiirePrdId", "ShohinId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ShiireMaster");

                    b.Navigation("TentoHaraidashiHeader");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.TentoZaiko", b =>
                {
                    b.HasOne("Convenience.Models.DataModels.ShohinMaster", "ShohinMaster")
                        .WithOne("TentoZaiko")
                        .HasForeignKey("Convenience.Models.DataModels.TentoZaiko", "ShohinId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ShohinMaster");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.ChumonJisseki", b =>
                {
                    b.Navigation("ChumonJissekiMeisais");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.ChumonJissekiMeisai", b =>
                {
                    b.Navigation("ShiireJisseki");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.KaikeiHeader", b =>
                {
                    b.Navigation("KaikeiJissekis");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.NaigaiClassMaster", b =>
                {
                    b.Navigation("KaikeiJissekis");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.ShiireMaster", b =>
                {
                    b.Navigation("ChumonJissekiMeisaiis");

                    b.Navigation("SokoZaiko");

                    b.Navigation("TentoHaraidashiJissekis");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.ShiireSakiMaster", b =>
                {
                    b.Navigation("ChumonJissekis");

                    b.Navigation("ShireMasters");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.ShohinMaster", b =>
                {
                    b.Navigation("ShiireMasters");

                    b.Navigation("TentoZaiko");
                });

            modelBuilder.Entity("Convenience.Models.DataModels.TentoHaraidashiHeader", b =>
                {
                    b.Navigation("TentoHaraidashiJissekis");
                });
#pragma warning restore 612, 618
        }
    }
}
