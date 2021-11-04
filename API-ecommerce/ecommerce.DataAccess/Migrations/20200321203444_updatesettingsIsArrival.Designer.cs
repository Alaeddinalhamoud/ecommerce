﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ecommerce.DataAccess;

namespace ecommerce.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20200321203444_updatesettingsIsArrival")]
    partial class updatesettingsIsArrival
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ecommerce.Data.Address", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("addressline1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("addressline2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("city")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("createDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("createdBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("houseNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.Property<double>("latitude")
                        .HasColumnType("float");

                    b.Property<double>("longitude")
                        .HasColumnType("float");

                    b.Property<string>("modifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("street")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("updateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("id");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("ecommerce.Data.Brand", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("createDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("createdBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("imagePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("modifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("updateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("id");

                    b.ToTable("Brands");
                });

            modelBuilder.Entity("ecommerce.Data.Cart", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("createDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("createdBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("modifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("status")
                        .HasColumnType("int");

                    b.Property<double>("total")
                        .HasColumnType("float");

                    b.Property<DateTime?>("updateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("id");

                    b.ToTable("Carts");
                });

            modelBuilder.Entity("ecommerce.Data.CartLine", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("cartId")
                        .HasColumnType("int");

                    b.Property<DateTime>("createDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("createdBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("modifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("price")
                        .HasColumnType("float");

                    b.Property<int>("productId")
                        .HasColumnType("int");

                    b.Property<int>("qty")
                        .HasColumnType("int");

                    b.Property<DateTime>("updateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("id");

                    b.ToTable("CartLines");
                });

            modelBuilder.Entity("ecommerce.Data.Category", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("createDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("createdBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("imagePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("modifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("updateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("ecommerce.Data.Media", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("alt")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("createDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("createdBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("mediaType")
                        .HasColumnType("int");

                    b.Property<string>("modifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("path")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("productId")
                        .HasColumnType("int");

                    b.Property<DateTime>("updateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("id");

                    b.ToTable("Medias");
                });

            modelBuilder.Entity("ecommerce.Data.Order", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("cartId")
                        .HasColumnType("int");

                    b.Property<DateTime>("createDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("createdBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("delivery")
                        .HasColumnType("float");

                    b.Property<string>("description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("isPaid")
                        .HasColumnType("bit");

                    b.Property<string>("modifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("paymentMethod")
                        .HasColumnType("int");

                    b.Property<double>("total")
                        .HasColumnType("float");

                    b.Property<DateTime>("updateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("ecommerce.Data.OrderLine", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("createDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("modifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("orderId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("price")
                        .HasColumnType("float");

                    b.Property<int>("productId")
                        .HasColumnType("int");

                    b.Property<int>("qty")
                        .HasColumnType("int");

                    b.Property<DateTime>("updateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("id");

                    b.ToTable("OrderLines");
                });

            modelBuilder.Entity("ecommerce.Data.Product", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("LotNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("barcode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("brandId")
                        .HasColumnType("int");

                    b.Property<int>("categoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("createDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("createdBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("expiryDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("frequency")
                        .HasColumnType("int");

                    b.Property<string>("inventoryCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isApproved")
                        .HasColumnType("bit");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("modelNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("modifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("placeOfOrigin")
                        .HasColumnType("int");

                    b.Property<double>("price")
                        .HasColumnType("float");

                    b.Property<int>("productTypeId")
                        .HasColumnType("int");

                    b.Property<int>("qty")
                        .HasColumnType("int");

                    b.Property<double>("rating")
                        .HasColumnType("float");

                    b.Property<string>("shortDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("tags")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("updateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("vendorId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("videoUrl")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("ecommerce.Data.ProductSpecification", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("createDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("createdBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("modifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("productId")
                        .HasColumnType("int");

                    b.Property<DateTime>("updateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("ProductSpecification");
                });

            modelBuilder.Entity("ecommerce.Data.ProductType", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("createDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("createdBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("modifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("updateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("id");

                    b.ToTable("ProductType");
                });

            modelBuilder.Entity("ecommerce.Data.RecentlyViewedProduct", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("createDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("productId")
                        .HasColumnType("int");

                    b.Property<DateTime>("updateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("userId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("RecentlyViewedProducts");
                });

            modelBuilder.Entity("ecommerce.Data.Review", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("createDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("createdBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isApproved")
                        .HasColumnType("bit");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("modifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("productId")
                        .HasColumnType("int");

                    b.Property<int>("rating")
                        .HasColumnType("int");

                    b.Property<DateTime>("updateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("id");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("ecommerce.Data.Setting", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("createDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("createdBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isMostViewdProduct")
                        .HasColumnType("bit");

                    b.Property<bool>("isNewArrivalProduct")
                        .HasColumnType("bit");

                    b.Property<bool>("isSlider")
                        .HasColumnType("bit");

                    b.Property<string>("logo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("modifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("numberOfSliders")
                        .HasColumnType("int");

                    b.Property<int>("pageSize")
                        .HasColumnType("int");

                    b.Property<string>("phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("updateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("websiteName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("ecommerce.Data.Slider", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("createDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("createdBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("imagePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("link")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("modifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("updateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("id");

                    b.ToTable("Sliders");
                });

            modelBuilder.Entity("ecommerce.Data.Wishlist", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("createDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("createdBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("modifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("productId")
                        .HasColumnType("int");

                    b.Property<DateTime>("updateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("userId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("WishLists");
                });
#pragma warning restore 612, 618
        }
    }
}
