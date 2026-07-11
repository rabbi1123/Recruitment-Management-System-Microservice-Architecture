using Common.Platform.Domain.Abstractions;

namespace Organization.Domain.Organization
{
	public class Organizations : IEntity
	{
		public Organizations(
			long id,
			string name,
			string? industry,
			string? website,
			string? email,
			string? phone,
			string? address,
			string? logoUrl,
			bool isActive,
			string? createdBy,
			DateTime? updatedOn,
			string? updatedBy)
		{
			Id = id;
			Name = name;
			Industry = industry;
			Website = website;
			Email = email;
			Phone = phone;
			Address = address;
			LogoUrl = logoUrl;
			IsActive = isActive;

			CreatedBy = createdBy;
			UpdatedOn = updatedOn;
			UpdatedBy = updatedBy;
		}

		public Organizations()
		{
		}

		public string GetPrimaryKeyName() => nameof(Id);

		public long GetPrimaryKeyValue() => Id;

		public long Id { get; set; }

		public string Name { get; set; }

		public string? Industry { get; set; }

		public string? Website { get; set; }

		public string? Email { get; set; }

		public string? Phone { get; set; }

		public string? Address { get; set; }

		public string? LogoUrl { get; set; }

		public bool IsActive { get; set; }

		public DateTime CreatedOn { get; set; }

		public string? CreatedBy { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public string? UpdatedBy { get; set; }

		public static Organizations Create(
			string name,
			string? industry,
			string? website,
			string? email,
			string? phone,
			string? address,
			string? logoUrl,
			string? createdBy)
		{
			return new Organizations(
				id: 0,
				name: name,
				industry: industry,
				website: website,
				email: email,
				phone: phone,
				address: address,
				logoUrl: logoUrl,
				isActive: true,
				createdBy: createdBy,
				updatedOn: null,
				updatedBy: null);
		}

		public void Update(
			string name,
			string? industry,
			string? website,
			string? email,
			string? phone,
			string? address,
			string? logoUrl,
			bool isActive,
			string? updatedBy)
		{
			Name = name;
			Industry = industry;
			Website = website;
			Email = email;
			Phone = phone;
			Address = address;
			LogoUrl = logoUrl;
			IsActive = isActive;

			UpdatedBy = updatedBy;
			UpdatedOn = DateTime.Now;
		}
	}
}
