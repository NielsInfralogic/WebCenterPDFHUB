using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;


namespace WebCenter4.Classes
{
	/// <summary>
	/// Define array of name/ID pairs
	/// </summary>
 
	public class NameItem
	{
		public int		nID;
		public String	sName;
	}

	public class NameItemCollection : System.Collections.CollectionBase
	{
		public void Add(NameItem aNameItem)
		{
			List.Add(aNameItem);
		}
		public void Remove(int index)
		{
			if (index > Count - 1 || index < 0)
			{
				//System.Windows.Forms.MessageBox.Show("Index not valid!");
			}
			else
			{
				List.RemoveAt(index); 
			}
		}
		public NameItem Item(int Index)
		{
			return (NameItem) List[Index];
		}

		public String FindName(int nID)
		{
			foreach (NameItem item in List)
			{
				if (item.nID == nID)
					return item.sName;
			}
			return "";
		}

		public int FindID(String sName)
		{
			foreach (NameItem item in List)
			{
				if (item.sName == sName)
					return item.nID;
			}
			return 0;
		}
	}


	public class NameItemEx
	{
		public int		nID;
		public int		nType;
		public String	sName;
	}

	public class NameItemCollectionEx : System.Collections.CollectionBase
	{
		public void Add(NameItemEx aNameItem)
		{
			List.Add(aNameItem);
		}
		public void Remove(int index)
		{
			if (index > Count - 1 || index < 0)
			{
				//System.Windows.Forms.MessageBox.Show("Index not valid!");
			}
			else
			{
				List.RemoveAt(index); 
			}
		}
		public NameItemEx Item(int Index)
		{
			return (NameItemEx) List[Index];
		}

		public String FindName(int nID)
		{
			foreach (NameItemEx item in List)
			{
				if (item.nID == nID)
					return item.sName;
			}
			return "";
		}

		public int FindID(String sName)
		{
			foreach (NameItemEx item in List)
			{
				if (item.sName == sName)
					return item.nID;
			}
			return 0;
		}

		public int FindType(int nID)
		{
			foreach (NameItemEx item in List)
			{
				if (item.nID == nID)
					return item.nType;
			}
			return 0;
		}
	}



	public class ColorItem
	{
		public int		ColorID;
		public String	ColorName;
		public int		c;
		public int		m;
		public int		y;
		public int		k;
		public int		ColorOrder;
	}
	public class ColorItemCollection : System.Collections.CollectionBase
	{
		public void Add(ColorItem aColorItem)
		{
			List.Add(aColorItem);
		}
		public void Remove(int index)
		{
			if (index > Count - 1 || index < 0)
			{
				//System.Windows.Forms.MessageBox.Show("Index not valid!");
			}
			else
			{
				List.RemoveAt(index); 
			}
		}
		public ColorItem Item(int Index)
		{
			return (ColorItem) List[Index];
		}

		public Color FindItemColor(String sColorName)
		{
			foreach (ColorItem item in List)
			{
				if (item.ColorName == sColorName)
				{
					sColorName.ToUpper();

					// Is this a known color?
					if (sColorName == "C" || sColorName == "CYAN" || sColorName == "DOUBLE BURN C")
						return Color.Cyan;
					if (sColorName == "M" || sColorName == "MAGENTA" || sColorName == "DOUBLE BURN M")
						return Color.Magenta;
					if (sColorName == "Y" || sColorName == "YELLOW" || sColorName == "DOUBLE BURN Y")
						return Color.Yellow;
					if (sColorName == "K" || sColorName == "BLACK" || sColorName == "DOUBLE BURN K")
						return Color.Black;
					

					// Unknown - find in spot table. Spot c,m,y,k percent values are coded to string "CCMMYYKK"
					int cy =255*100 - 100* item.c;
					int m = 255*100 - 100* item.m;
					int y = 255*100 - 100* item.y;
					int k = 255*100 - 100* item.k;
					int r = cy*k/100;
					int gr = m*k/100;
					int b = y*k/100;
					return Color.FromArgb(r/(255*100), gr/(255*100), b/(255*100));
				
				}
			}
			return Color.White;
		}

		public String FindColor(int nID)
		{
			foreach (ColorItem item in List)
			{
				if (item.ColorID == nID)
					return item.ColorName;
			}
			return "";
		}

		public int FindID(String sName)
		{
			foreach (ColorItem item in List)
			{
				if (item.ColorName == sName)
					return item.ColorID;
			}
			return 0;
		}
	}

	public class StatusItem
	{
		public String	StatusName;
		public int		StatusNumber;
		public Color	StatusColor;
	}
	public class StatusItemCollection : System.Collections.CollectionBase
	{
		public void Add(StatusItem aStatusItem)
		{
			List.Add(aStatusItem);
		}
		public void Remove(int index)
		{
			if (index > Count - 1 || index < 0)
			{
				//System.Windows.Forms.MessageBox.Show("Index not valid!");
			}
			else
			{
				List.RemoveAt(index); 
			}
		}
		public StatusItem Item(int Index)
		{
			return (StatusItem) List[Index];
		}

		public StatusItem FindItem(int nCode)
		{
			foreach (StatusItem item in List)
			{
				if (item.StatusNumber == nCode)
					return item;
			}
			return null;
		}

		public Color FindItemColor(String sStatusName)
		{
			foreach (StatusItem item in List)
			{
				if (item.StatusName == sStatusName)
					return item.StatusColor;
			}
			return Color.White;
		}

	}

	public class ProductionItem
	{
		public int		ProductionID;
		public int		PublicationID;
		public DateTime Pubdate;
		public int		IssueID;
		public int		EditionID;
		public int		SectionID;
		public int		PressID;
		public int		LocationID;
		public int		PressRunID;
		public int		Hold;
	}
	public class ProductionItemCollection : System.Collections.CollectionBase
	{
		public void Add(ProductionItem aProductionItem)
		{
			List.Add(aProductionItem);
		}
		public void Remove(int index)
		{
			if (index > Count - 1 || index < 0)
			{
				//System.Windows.Forms.MessageBox.Show("Index not valid!");
			}
			else
			{
				List.RemoveAt(index); 
			}
		}
		public ProductionItem Item(int Index)
		{
			return (ProductionItem) List[Index];
		}
	}

	public class PageSetNumberItem
	{
		public int	CopySeparationSet;
		public int	SeparationSet;
		public int	Separation;
		public int	CopyFlatSeparationSet;
		public int	FlatSeparationSet;
		public int	FlatSeparation;
	}

	public class PageTableItem
	{
		public int		CopySeparationSet;
		public int		SeparationSet;
		public int		Separation;
		public int		CopyFlatSeparationSet;
		public int		FlatSeparationSet;
		public int		FlatSeparation;
		public int		Status;
		public int		ExternalStatus;

		public int		ProductionID;
		public int		PublicationID;
		public DateTime Pubdate;
		public int		IssueID;
		public int		EditionID;
		public int		SectionID;
		public int		PressID;
		public int		PressRunID;

		public string	PageName;
		public string	ColorName;

		public int		TemplateID;
		public int		ProofID;
		public int		DeviceID;

		public int		Version;
		public int		Layer;
		public int		CopyNumber;
		public int		Pagination;

		public int		Approved;
		public int		Hold;
		public int		Priority;
		public int		PagePosition;
		public int		PageType;
		public int		PagesOnPlate;
		public int		SheetNumber;
		public int		SheetSide;
		public int		PressSectionNumber;
		public int		SortingPosition;

		public DateTime	InputTime;
		public DateTime	ApproveTime;
		public DateTime	ReadyTime;
		public DateTime	OutputTime;
		public DateTime	VerifyTime;
		public string	ApproveUser;
		public string	FileName;
		public string	LastError;
		public string	Comment;
		public DateTime	DeadLine;

		public int		ColorOrder;

		public int		ProofStatus;
		public int		InkStatus;

	}
	public class PageTableItemCollection : System.Collections.CollectionBase
	{
		public void Add(PageTableItem aPageTableItem)
		{
			List.Add(aPageTableItem);
		}
		public void Remove(int index)
		{
			if (index > Count - 1 || index < 0)
			{
				//System.Windows.Forms.MessageBox.Show("Index not valid!");
			}
			else
			{
				List.RemoveAt(index); 
			}
		}
		public PageTableItem Item(int Index)
		{
			return (PageTableItem) List[Index];
		}
	}

	public class ProdItem
	{
		public int productionID;
		public int publicationID;
		public DateTime pubDate;

		public ProdItem()
		{
		}

		public ProdItem(int productionID, int publicationID, DateTime pubDate)
		{
			this.productionID = productionID;
			this.publicationID = publicationID;
			this.pubDate = pubDate;
		}
	}

	public class ProdList : System.Collections.CollectionBase
	{

		public void Add(int productionID, int publicationID, DateTime pubDate)
		{
			List.Add(new ProdItem(productionID, publicationID, pubDate));
		}

		public void Add(ProdItem prodItem)
		{
			List.Add(prodItem);
		}

		public void Remove(int index)
		{
			if (index > Count - 1 || index < 0)
			{
				//System.Windows.Forms.MessageBox.Show("Index not valid!");
			}
			else
			{
				List.RemoveAt(index); 
			}
		}

		public ProdItem Item(int Index)
		{
			return (ProdItem) List[Index];
		}


		public int FindProductionID(int publicationID, DateTime pubDate)
		{
			foreach (ProdItem item in List)
			{
				if (item.publicationID == publicationID && item.pubDate == pubDate)
					return item.productionID;
			}
			return 0;
		}

		public bool FindPubAndPubDate(int productionID, out int publicationID, out DateTime pubDate)
		{

			publicationID = 0;
			pubDate = DateTime.MinValue;

			foreach (ProdItem item in List)
			{
				if (item.productionID == productionID)
				{
					publicationID = item.publicationID;
					pubDate = item.pubDate;
					return true;
				}
			}
			return false;

		}
	}

	public class NameCacheItems 
	{
		/// <summary>
		/// Generel structure with (mostly) static ID arrays loaded at startup
		/// </summary>
		public NameItemCollection	EditionNames = new NameItemCollection();
		public NameItemCollection	PublicationNames = new NameItemCollection();
		public NameItemCollection	SectionNames = new NameItemCollection();
		public NameItemCollection	LocationNames = new NameItemCollection();
		public NameItemCollection	PressNames = new NameItemCollection();
		public NameItemCollection	IssueNames = new NameItemCollection();

		public NameItemCollection	DeviceNames = new NameItemCollection();
		public StatusItemCollection StatusNames = new StatusItemCollection();
		public StatusItemCollection ExternalStatusNames = new StatusItemCollection();

		public ColorItemCollection	ColorNames = new ColorItemCollection();
		/// <summary>
		/// Production structure is loaded at intervals
		/// </summary>
		public NameItemCollection	ProductionNames = new NameItemCollection();
	}

	/// <summary>
	/// Summary description for NameCaches.
	/// </summary>
	public class NameCaches
	{
		NameCacheItems aNameCache = new NameCacheItems();
		
		public NameCaches()
		{
		}

	/*	public bool UpdateCache(out string errmsg)
		{
			errmsg = "";
			CCDBaccess db = new CCDBaccess();

			if (DB.GetIDCollection("GetPublicationNames", aNameCache.PublicationNames, out errmsg) == false)
			{
				return false;
			}
			if (DB.GetIDCollection("GetEditionNames", aNameCache.EditionNames, out errmsg) == false)
			{
				return false;
			}			
			if (DB.GetIDCollection("GetSectionNames", aNameCache.SectionNames, out errmsg) == false)
			{
				return false;
			}				
			if (DB.GetIDCollection("GetIssueNames", aNameCache.IssueNames, out errmsg) == false)
			{
				return false;
			}				
			if (DB.GetIDCollection("GetPressNames", aNameCache.PressNames, out errmsg) == false)
			{
				return false;
			}
			if (DB.GetIDCollection("GetLocationNames", aNameCache.LocationNames, out errmsg) == false)
			{
				return false;
			}	
			if (DB.GetIDCollection("GetDeviceNames", aNameCache.DeviceNames,  out errmsg) == false)
			{
				return false;
			}	

			if (DB.GetColorCollection(aNameCache.ColorNames,  out errmsg) == false)
			{
				return false;
			}	

			if (DB.GetStatusCollection(0, aNameCache.StatusNames,  out errmsg) == false)
			{
				return false;
			}	

			if (DB.GetStatusCollection(1, aNameCache.ExternalStatusNames,  out errmsg) == false)
			{
				return false;
			}	

			DB.CloseAll();
			return true;

		}

		public NameCacheItems GetCache()
		{
			return aNameCache;
		}*/
		
	}



	




}
