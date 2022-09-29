public partial class SOShipmentEntry : PXGraph<SOShipmentEntry, SOShipment>, IGraphWithInitialization
	{
		private DiscountEngine<SOShipLine, SOShipmentDiscountDetail> _discountEngine => DiscountEngineProvider.GetEngineFor<SOShipLine, SOShipmentDiscountDetail>();
		public SOShipmentLineSplittingExtension LineSplittingExt => FindImplementation<SOShipmentLineSplittingExtension>();
		public SOShipmentItemAvailabilityExtension ItemAvailabilityExt => FindImplementation<SOShipmentItemAvailabilityExtension>();

		public ToggleCurrency<SOShipment> CurrencyView;
		[PXViewName(Messages.SOShipment)]
		public PXSelectJoin<SOShipment,
			LeftJoinSingleTable<Customer, On<Customer.bAccountID, Equal<SOShipment.customerID>>,
			LeftJoin<INSite, 
				On<SOShipment.FK.Site>>>,
			Where2<Where<Customer.bAccountID, IsNull,
			Or<Match<Customer, Current<AccessInfo.userName>>>>,
			And<Where<INSite.siteID, IsNull,
			Or<Match<INSite, Current<AccessInfo.userName>>>>>>> Document;
		public PXSelect<SOShipment, Where<SOShipment.shipmentNbr, Equal<Current<SOShipment.shipmentNbr>>>> CurrentDocument;
		public PXSelect<SOShipLine, Where<SOShipLine.shipmentNbr, Equal<Current<SOShipment.shipmentNbr>>>, OrderBy<Asc<SOShipLine.shipmentNbr, Asc<SOShipLine.sortOrder>>>> Transactions;
		public PXSelect<SOShipLineSplit, Where<SOShipLineSplit.shipmentNbr, Equal<Current<SOShipLine.shipmentNbr>>, And<SOShipLineSplit.lineNbr, Equal<Current<SOShipLine.lineNbr>>>>> splits;
		public PXSelect<Unassigned.SOShipLineSplit, Where<Unassigned.SOShipLineSplit.shipmentNbr, Equal<Current<SOShipLine.shipmentNbr>>, And<Unassigned.SOShipLineSplit.lineNbr, Equal<Current<SOShipLine.lineNbr>>>>> unassignedSplits;
		[PXViewName(Messages.ShippingAddress)]
		public PXSelect<SOShipmentAddress, Where<SOShipmentAddress.addressID, Equal<Current<SOShipment.shipAddressID>>>> Shipping_Address;
		[PXViewName(Messages.ShippingContact)]
		public PXSelect<SOShipmentContact, Where<SOShipmentContact.contactID, Equal<Current<SOShipment.shipContactID>>>> Shipping_Contact;
		[PXViewName(Messages.SOOrderShipment)]
		public PXSelectJoin<SOOrderShipment,
				InnerJoin<SOOrder, On<SOOrder.orderType, Equal<SOOrderShipment.orderType>, And<SOOrder.orderNbr, Equal<SOOrderShipment.orderNbr>>>,
				InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<SOOrder.curyInfoID>>,
				InnerJoin<SOAddress, On<SOAddress.addressID, Equal<SOOrder.billAddressID>>,
				InnerJoin<SOContact, On<SOContact.contactID, Equal<SOOrder.billContactID>>>>>>,
				Where<SOOrderShipment.shipmentNbr, Equal<Current<SOShipment.shipmentNbr>>, And<SOOrderShipment.shipmentType, Equal<Current<SOShipment.shipmentType>>>>> OrderList;

		public PXSelect<SOOrder,
			Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
				And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>
			soorder;
		public PXSetup<SOOrderType, Where<SOOrderType.orderType, Equal<Optional<SOOrder.orderType>>>> soordertype;
		public PXSelect<SOSetupApproval> sosetupapproval;
		public EPApprovalAutomation<SOOrder, SOOrder.approved, SOOrder.rejected, SOOrder.hold, SOSetupApproval> Approval;

		public PXSelect<SOOrderShipment, Where<SOOrderShipment.shipmentNbr, Equal<Current<SOShipment.shipmentNbr>>, And<SOOrderShipment.shipmentType, Equal<Current<SOShipment.shipmentType>>>>> OrderListSimple;
		public PXSelect<SOShipmentDiscountDetail, Where<SOShipmentDiscountDetail.shipmentNbr, Equal<Current<SOShipment.shipmentNbr>>>> DiscountDetails;
		public PXSelect<SOShipLine, Where<SOShipLine.shipmentNbr, Equal<Current<SOShipment.shipmentNbr>>, And<SOShipLine.isFree, Equal<boolTrue>>>, OrderBy<Asc<SOShipLine.shipmentNbr, Asc<SOShipLine.lineNbr>>>> FreeItems;
		[PXViewName(Messages.SOPackageDetail)]
		public PXSelect<SOPackageDetailEx, Where<SOPackageDetailEx.shipmentNbr, Equal<Current<SOShipment.shipmentNbr>>>> Packages;
		[PXHidden]
		public PXSelect<SOPackageDetailEx, Where<SOPackageDetailEx.shipmentNbr, Equal<Current<SOShipment.shipmentNbr>>>> PackagesForRates;
		[PXHidden]
		[PXCopyPasteHiddenView]
		public PXSelect<CarrierLabelHistory> LabelHistory;
		public PXSetup<Carrier, Where<Carrier.carrierID, Equal<Current<SOShipment.shipVia>>>> carrier;
		public PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<SOShipment.curyInfoID>>>> currencyinfo;
		public PXSelect<CurrencyInfo> DummyCuryInfo;
		public SelectFrom<SOShipmentProcessedByUser>.View ShipmentWorkLog;

		public PXSetup<INSetup> insetup;
		public PXSetup<SOSetup> sosetup;
		public PXSetup<ARSetup> arsetup;
		public PXSetupOptional<CommonSetup> commonsetup;

		public PXSetup<GL.Branch, InnerJoin<INSite, On<INSite.branchID, Equal<GL.Branch.branchID>>>, Where<INSite.siteID, Equal<Optional<SOShipment.destinationSiteID>>, And<Current<SOShipment.shipmentType>, Equal<SOShipmentType.transfer>, Or<INSite.siteID, Equal<Optional<SOShipment.siteID>>, And<Current<SOShipment.shipmentType>, NotEqual<SOShipmentType.transfer>>>>>> Company; //TODO: Need review INRegister Branch and SOShipment SiteID/DestinationSiteID AC-55773
		public PXSetup<Customer, Where<Customer.bAccountID, Equal<Optional<SOShipment.customerID>>>> customer;
		public PXSetup<Location, Where<Location.bAccountID, Equal<Current<SOShipment.customerID>>, And<Location.locationID, Equal<Optional<SOShipment.customerLocationID>>>>> location;

		public PXSelect<SOLine2> soline;
		public PXSelect<SOLineSplit2> solinesplit;
		public PXSelect<SOLine> dummy_soline; //will prevent collection was modified if no Select<SOLine> was executed prior to Persist()

		public PXFilter<AddSOFilter> addsofilter;
        public PXSelectJoinOrderBy<SOShipmentPlan,
               InnerJoin<SOLineSplit,
               On<SOLineSplit.planID, Equal<SOShipmentPlan.planID>>,
               InnerJoin<SOLine,
               On<SOLine.orderType, Equal<SOLineSplit.orderType>, And<SOLine.orderNbr, Equal<SOLineSplit.orderNbr>, And<SOLine.lineNbr, Equal<SOLineSplit.lineNbr>>>>>>,
			   OrderBy<Asc<SOLine.sortOrder, Asc<SOLine.lineNbr, Asc<SOLineSplit.lineNbr>>>>> soshipmentplan;

		[CRReference(typeof(Select<Customer, Where<Customer.bAccountID, Equal<Current<SOShipment.customerID>>>>))]
		[CRDefaultMailTo(typeof(Select<SOShipmentContact, Where<SOShipmentContact.contactID, Equal<Current<SOShipment.shipContactID>>>>))]
		public CRActivityList<SOShipment>
			Activity;

		[PXViewName(CR.Messages.MainContact)]
		public PXSelect<Contact> DefaultCompanyContact;
		protected virtual IEnumerable defaultCompanyContact()
		{
			return OrganizationMaint.GetDefaultContactForCurrentOrganization(this);
		}

		[PXCopyPasteHiddenView()]
		public SelectFrom<SOOrderSite>
			.InnerJoin<SOOrderShipment>.On<SOOrderShipment.FK.OrderSite>
			.Where<SOOrderShipment.FK.Shipment.SameAsCurrent>.View OrderSite;

		public PXAction<SOShipment> putOnHold;
		[PXButton(CommitChanges = true), PXUIField(DisplayName = "Hold", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		protected virtual IEnumerable PutOnHold(PXAdapter adapter) => adapter.Get();

		public PXAction<SOShipment> releaseFromHold;
		[PXButton(CommitChanges = true), PXUIField(DisplayName = "Remove Hold")]
		protected virtual IEnumerable ReleaseFromHold(PXAdapter adapter) => adapter.Get();

		public PXInitializeState<SOShipment> initializeState;

		public PXAction<SOShipment> notification;
		[PXUIField(DisplayName = "Notifications", Visible = false)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntryF)]
		protected virtual IEnumerable Notification(PXAdapter adapter,
		[PXString]
		string notificationCD
		)
		{
			foreach (SOShipment shipment in adapter.Get<SOShipment>())
			{
				Document.Current = shipment;

				var parameters = new Dictionary<string, string>();
				parameters["SOShipment.ShipmentNbr"] = shipment.ShipmentNbr;

				GL.Branch branch = PXSelectReadonly2<GL.Branch, InnerJoin<INSite, On<INSite.branchID, Equal<GL.Branch.branchID>>>,
						Where<INSite.siteID, Equal<Optional<SOShipment.destinationSiteID>>,
								And<Current<SOShipment.shipmentType>, Equal<SOShipmentType.transfer>, 
							Or<INSite.siteID, Equal<Optional<SOShipment.siteID>>,
								And<Current<SOShipment.shipmentType>, NotEqual<SOShipmentType.transfer>>>>>>
					.SelectSingleBound(this, new object[] {shipment});
				
				Activity.SendNotification(ARNotificationSource.Customer, notificationCD, (branch != null && branch.BranchID != null) ? branch.BranchID : Accessinfo.BranchID, parameters, adapter.MassProcess);

				yield return shipment;
			}
		}

		public PXAction<SOShipment> emailShipment;
		[PXButton(CommitChanges = true), PXUIField(DisplayName = "Email Shipment", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		public virtual IEnumerable EmailShipment(
			PXAdapter adapter,
			[PXString]
			string notificationCD = null) => Notification(adapter, notificationCD ?? "SHIPMENT");

		[Obsolete(Common.Messages.ItemIsObsoleteAndWillBeRemoved2022R2)]
		public PXMenu<SOShipment> action;
		[PXUIField(DisplayName = "Actions", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton(CommitChanges = true, SpecialType = PXSpecialButtonType.ActionsFolder, MenuAutoOpen = true)]
		[Obsolete(Common.Messages.ItemIsObsoleteAndWillBeRemoved2022R2)]
		protected virtual IEnumerable Action(PXAdapter adapter,
			[PXInt]
			[ShipmentActions]
			int? actionID,
			[PXString()]
			string ActionName
			) => action.Press(adapter, actionID, ActionName);

		#region Action menu items

		public PXAction<SOShipment> inquiry;
		[PXUIField(DisplayName = "Inquiries", MapEnableRights = PXCacheRights.Select)]
		[PXButton(SpecialType = PXSpecialButtonType.InquiriesFolder, MenuAutoOpen = true)]
		protected virtual IEnumerable Inquiry(PXAdapter adapter,
			[PXInt]
			[PXIntList(new int[] { }, new string[] { })]
			int? inquiryID,
			[PXString()]
			string ActionName
			)
		{
			if (!string.IsNullOrEmpty(ActionName))
			{
				PXAction action = this.Actions[ActionName];

				if (action != null)
				{
					Save.Press();
					foreach (object data in action.Press(adapter)) ;
				}
			}
			return adapter.Get();
		}

		//throw new PXReportRequiredException(parameters, "SO642000", "Shipment Confirmation");
		public PXAction<SOShipment> report;
		[PXUIField(DisplayName = "Reports", MapEnableRights = PXCacheRights.Select)]
		[PXButton(SpecialType = PXSpecialButtonType.ReportsFolder, MenuAutoOpen = true)]
		public virtual IEnumerable Report(PXAdapter adapter, [PXString(8, InputMask = "CC.CC.CC.CC")] string reportID)
		{
			var shipments = adapter.Get<SOShipment>().ToImmutableList();
			if (!String.IsNullOrEmpty(reportID) && shipments.Any())
			{
				Save.Press();

				string GetActualReportID(SOShipment shipment)
				{
					Document.Current = shipment;
					GL.Branch company = null;
					using (new PXReadBranchRestrictedScope()) company = Company.Select();
					object customer = PXSelectorAttribute.Select<SOShipment.customerID>(Document.Cache, shipment);
					return new NotificationUtility(this).SearchReport(SONotificationSource.Customer, customer, reportID, company.BranchID);
				}

				PXReportRequiredException combinedReport = shipments
					.Select(sh =>
					(
						ActualReportID: GetActualReportID(sh),
						Parameters: new Dictionary<string, string> { ["SOShipment.ShipmentNbr"] = sh.ShipmentNbr }
					))
					.Aggregate(
						(PXReportRequiredException)null,
						(acc, elem) =>
						{
							var report = PXReportRequiredException.CombineReport(acc, elem.ActualReportID, elem.Parameters);
							report.Mode = PXBaseRedirectException.WindowMode.New;
							return report;
						});

				if (combinedReport != null)
				{
					if (PXAccess.FeatureInstalled<FeaturesSet.deviceHub>())
						SMPrintJobMaint.CreatePrintJobGroup(adapter, new NotificationUtility(this).SearchPrinter, SONotificationSource.Customer, reportID, Accessinfo.BranchID, combinedReport, null);

					throw combinedReport;
				}
			}
			return shipments;
		}

		public PXAction<SOShipment> printShipmentConfirmation;
		[PXButton(CommitChanges = true), PXUIField(DisplayName = "Print Shipment Confirmation", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		public virtual IEnumerable PrintShipmentConfirmation(PXAdapter adapter) => Report(adapter.Apply(it => it.Menu = "Print Shipment Confirmation"), "SO642000");

		public virtual void SOShipmentPlan_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (e.Row == null) return;

			SOShipmentPlan plan = (SOShipmentPlan)e.Row;
			if (Document.Current.ShipDate < plan.PlanDate)
			{
				PXUIFieldAttribute.SetWarning<SOShipmentPlan.planDate>(sender, plan, Messages.PlanDateGreaterShipDate);
			}
		}

		public PXSelectJoin<SOShipmentPlan,
					InnerJoin<SOLineSplit, On<SOLineSplit.planID, Equal<SOShipmentPlan.planID>>,
					LeftJoin<SOOrderShipment,
						On<SOOrderShipment.orderType, Equal<SOShipmentPlan.orderType>,
							And<SOOrderShipment.orderNbr, Equal<SOShipmentPlan.orderNbr>,
							And<SOOrderShipment.operation, Equal<SOLineSplit.operation>,
							And<SOOrderShipment.siteID, Equal<SOShipmentPlan.siteID>,
							And<SOOrderShipment.confirmed, Equal<boolFalse>,
							And<SOOrderShipment.shipmentNbr, NotEqual<Current<SOShipment.shipmentNbr>>>>>>>>,
					LeftJoin<SOLine,
						On<SOLineSplit.orderType, Equal<SOLine.orderType>,
							And<SOLineSplit.orderNbr, Equal<SOLine.orderNbr>,
							And<SOLineSplit.lineNbr, Equal<SOLine.lineNbr>>>>>>>,
					Where<SOShipmentPlan.orderType, Equal<Current<AddSOFilter.orderType>>,
						And<SOShipmentPlan.orderNbr, Equal<Current<AddSOFilter.orderNbr>>,
						And<SOShipmentPlan.siteID, Equal<Current<SOShipment.siteID>>,
						And<SOOrderShipment.shipmentNbr, IsNull,
						And<SOLineSplit.operation, Equal<Current<AddSOFilter.operation>>,
						And2<Where<Current<SOShipment.destinationSiteID>, IsNull,
							Or<SOShipmentPlan.destinationSiteID, Equal<Current<SOShipment.destinationSiteID>>>>,
						And2<Where<SOShipmentPlan.inclQtySOShipping, Equal<True>,
							Or<SOShipmentPlan.inclQtySOShipped, Equal<True>,
							Or2<Where<SOShipmentPlan.requireAllocation, Equal<False>, Or<SOLineSplit.operation, Equal<SOOperation.receipt>>>,
							Or<SOLineSplit.lineType, Equal<SOLineType.nonInventory>>>>>,
						And<Where<Current<SOShipment.isManualPackage>, IsNull,
							Or<SOShipmentPlan.isManualPackage, Equal<Current<SOShipment.isManualPackage>>>>>>>>>>>>> sOshipmentplanSelect;
							
		public virtual IEnumerable sOshipmentplan()
		{
			string shipmentFreightSrc = this.Document.Current?.FreightAmountSource,
				orderFreightSrc = this.addsofilter.Current?.FreightAmountSource;
			if (!shipmentFreightSrc.IsIn(null, this.addsofilter.Current?.FreightAmountSource))
				yield break;

			var shipmentSOLineSplits = new Lazy<OrigSOLineSplitSet>(() => CollectShipmentOrigSOLineSplits());

			foreach (PXResult<SOShipmentPlan, SOLineSplit, SOOrderShipment, SOLine> res in
					sOshipmentplanSelect.Select())
			{
				SOLineSplit sls = (SOLineSplit)res;
				if (!shipmentSOLineSplits.Value.Contains(sls))
				{
					yield return new PXResult<SOShipmentPlan, SOLineSplit, SOLine>((SOShipmentPlan)res, sls, (SOLine)res);
				}
			}
		}

		protected virtual OrigSOLineSplitSet CollectShipmentOrigSOLineSplits()
		{
			var ret = new OrigSOLineSplitSet();
			PXSelectBase<SOShipLine> cmd = new PXSelectReadonly<SOShipLine, Where<SOShipLine.shipmentNbr, Equal<Current<SOShipment.shipmentNbr>>>>(this);
			using (new PXFieldScope(cmd.View, typeof(SOShipLine.shipmentNbr), typeof(SOShipLine.lineNbr),
					typeof(SOShipLine.origOrderType), typeof(SOShipLine.origOrderNbr), typeof(SOShipLine.origLineNbr), typeof(SOShipLine.origSplitLineNbr)))
			{
				foreach (SOShipLine sl in cmd.Select())
				{
					ret.Add(sl);
				}
			}
			foreach (SOShipLine sl in Transactions.Cache.Deleted)
			{
				ret.Remove(sl);
			}
			foreach (SOShipLine sl in Transactions.Cache.Inserted)
			{
				ret.Add(sl);
			}
			return ret;
		}

		public PXAction<SOShipment> inventorySummary;
		[PXUIField(DisplayName = "Inventory Summary", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable InventorySummary(PXAdapter adapter)
		{
			PXCache tCache = Transactions.Cache;
			SOShipLine line = Transactions.Current;
			if (line == null) return adapter.Get();

			InventoryItem item = InventoryItem.PK.Find(this, line.InventoryID);
			if (item != null && item.StkItem == true)
			{
				INSubItem sbitem = (INSubItem)PXSelectorAttribute.Select<SOShipLine.subItemID>(tCache, line);
				InventorySummaryEnq.Redirect(item.InventoryID,
											 ((sbitem != null) ? sbitem.SubItemCD : null),
											 line.SiteID,
											 line.LocationID);
			}
			return adapter.Get();
		}

		public SOShipmentEntry()
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.inventory>())
			{
				INSetup inrecord = insetup.Current;
			}

			CommonSetup csrecord = commonsetup.Current;
			SOSetup sorecord = sosetup.Current;


			ARSetupNoMigrationMode.EnsureMigrationModeDisabled(this);

			CopyPaste.SetVisible(false);
			PXDBDefaultAttribute.SetDefaultForInsert<SOOrderShipment.shipAddressID>(OrderList.Cache, null, true);
			PXDBDefaultAttribute.SetDefaultForUpdate<SOOrderShipment.shipAddressID>(OrderList.Cache, null, true);

			PXDBDefaultAttribute.SetDefaultForInsert<SOOrderShipment.shipContactID>(OrderList.Cache, null, true);
			PXDBDefaultAttribute.SetDefaultForUpdate<SOOrderShipment.shipContactID>(OrderList.Cache, null, true);

			PXDBLiteDefaultAttribute.SetDefaultForInsert<SOOrderShipment.shipmentNbr>(OrderList.Cache, null, true);
			PXDBLiteDefaultAttribute.SetDefaultForUpdate<SOOrderShipment.shipmentNbr>(OrderList.Cache, null, true);

			PXUIFieldAttribute.SetDisplayName<Contact.salutation>(Caches[typeof(Contact)], CR.Messages.Attention);
			this.Views.Caches.Add(typeof(SOLineSplit));

			FieldDefaulting.AddHandler<BAccountR.type>((sender, e) => { if (e.Row != null) e.NewValue = BAccountType.CustomerType; });
		}

		#region Entity Event Handlers

		[InjectDependency]
		protected ILicenseLimitsService _licenseLimits { get; set; }

		void IGraphWithInitialization.Initialize()
		{
			if (_licenseLimits != null)
			{
				OnBeforeCommit += _licenseLimits.GetCheckerDelegate<SOShipment>(
					new TableQuery(TransactionTypes.LinesPerMasterRecord, typeof(SOShipLine), (graph) =>
					{
						return new PXDataFieldValue[]
						{
							new PXDataFieldValue<SOShipLine.shipmentNbr>(((SOShipmentEntry)graph).Document.Current?.ShipmentNbr)
						};
					}),
					new TableQuery(TransactionTypes.SerialsPerDocument, typeof(SOShipLineSplit), (graph) =>
					{
						return new PXDataFieldValue[]
						{
							new PXDataFieldValue<SOShipLineSplit.shipmentNbr>(((SOShipmentEntry)graph).Document.Current?.ShipmentNbr)
						};
					}),
					new TableQuery(TransactionTypes.LinesPerMasterRecord, typeof(SOPackageDetail), (graph) =>
					{
						return new PXDataFieldValue[]
						{
							new PXDataFieldValue<SOPackageDetail.shipmentNbr>(((SOShipmentEntry)graph).Document.Current?.ShipmentNbr)
						};
					}));
			}
		}

		public PXAction<SOShipment> selectSO;
		[PXUIField(DisplayName = "Add Order", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable SelectSO(PXAdapter adapter)
		{
			if (this.Document.Cache.AllowDelete)
				addsofilter.AskExt();
			return adapter.Get();
		}

		public PXAction<SOShipment> addSO;
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXLookupButton]
		public virtual IEnumerable AddSO(PXAdapter adapter)
		{
			SOOrder order = PXSelect<SOOrder,
				Where<SOOrder.orderType, Equal<Optional<AddSOFilter.orderType>>,
					And<SOOrder.orderNbr, Equal<Optional<AddSOFilter.orderNbr>>>>>.Select(this);

			bool selected = order != null &&
				(addsofilter.Current?.AddAllLines == true || AnySelected<SOShipmentPlan.selected>(soshipmentplan.Cache));

			if (selected)
			{
				try
				{
					using (LineSplittingExt.ForceUnattendedModeScope(true))
						CreateShipment(new CreateShipmentArgs
						{
							MassProcess = false,
							Order = order,
							SiteID = Document.Current.SiteID,
							ShipDate = Document.Current.ShipDate,
							UseOptimalShipDate = false,
							Operation = addsofilter.Current.Operation,
							ShipmentList = addsofilter.Current.AddAllLines == true ? new DocumentList<SOShipment>(this) : null,
						});
				}
				finally
				{
					addsofilter.Current.AddAllLines = false;
				}

			}

			if (addsofilter.Current != null && !IsImport)
			{
				try
				{
					addsofilter.Cache.SetDefaultExt<AddSOFilter.orderType>(addsofilter.Current);
					addsofilter.Current.OrderNbr = null;
				}
				catch { }
			}

			soshipmentplan.Cache.Clear();
			soshipmentplan.View.Clear();
			soshipmentplan.Cache.ClearQueryCacheObsolete();
			sOshipmentplanSelect.View.Clear();
			ShipmentScheduleSelect.View.Clear();

			return adapter.Get();
		}

		public PXAction<SOShipment> addSOCancel;
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXLookupButton]
		public virtual IEnumerable AddSOCancel(PXAdapter adapter)
		{
			addsofilter.Cache.SetDefaultExt<AddSOFilter.orderType>(addsofilter.Current);
			addsofilter.Current.OrderNbr = null;
			soshipmentplan.Cache.Clear();
			soshipmentplan.View.Clear();

			return adapter.Get();
		}

		#region SOOrder Events

		#region SOLine CacheAttached

		#region SOShipLine Cache Attached

		#region SOShipmentAddress Cache Attached

		#region SOLine2 Events

		public PXAction<SOShipment> validateAddresses;
		[PXButton(CommitChanges = true), PXUIField(DisplayName = CS.Messages.ValidateAddresses, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, FieldClass = CS.Messages.ValidateAddress)]
		public virtual IEnumerable ValidateAddresses(PXAdapter adapter)
		{
			foreach (SOShipment current in adapter.Get<SOShipment>())
			{
				if (current != null)
				{
					SOShipmentAddress address = this.Shipping_Address.Select();
					if (address != null && address.IsDefaultAddress == false && address.IsValidated == false)
					{
						PXAddressValidator.Validate<SOShipmentAddress>(this, address, true, true);
					}
				}
				yield return current;
			}
		}

		#region CurrencyInfo events

		#region SOShipment Events

		#region SOOrderShipment Events

		#region SOShipLine Events

		#region SOShipLineSplit Events

		#region AddSOFilter Events

		#region SOPackageDetail Events

		#region SOShipmentContact Events

		#region Processing

		#region Discount

		#region Packaging into boxes

		protected virtual bool SyncLineWithOrder(SOShipLine row)
		{
			if (row.ShippedQty == 0)
			{
				var soLine = PXParentAttribute.SelectParent<SOLine2>(Transactions.Cache, row);
				if (soLine != null)
					return soLine.SiteID == row.SiteID;
			}
			return true;
		}

		protected virtual void CheckLocationTaskRule(PXCache sender, SOShipLine row)
		{
			if (row.TaskID != null)
			{
				INLocation selectedLocation = INLocation.PK.Find(this, row.LocationID);

				if (selectedLocation != null && selectedLocation.TaskID != row.TaskID && selectedLocation.TaskID != null)
				{
					sender.RaiseExceptionHandling<SOShipLine.locationID>(row, selectedLocation.LocationCD,
						new PXSetPropertyException(IN.Messages.LocationIsMappedToAnotherTask, PXErrorLevel.Warning));
				}
			}
		}
		
		protected virtual void CheckSplitsForSameTask(PXCache sender, SOShipLine row)
		{
            if (row.HasMixedProjectTasks == true)
            {
                sender.RaiseExceptionHandling<SOShipLine.locationID>(row, null, new PXSetPropertyException(IN.Messages.MixedProjectsInSplits));
            }

        }

		public virtual void ShipPackages(SOShipment shiporder)
		{
			var carrier = Carrier.PK.Find(this, shiporder.ShipVia);
			if (!UseCarrierService(shiporder, carrier))
				return;
			
			CarrierPlugin plugin = null;

			if (carrier.IsExternal == true)
			{
				plugin = CarrierPlugin.PK.Find(this, carrier.CarrierPluginID);
				if (plugin?.SiteID != null && plugin.SiteID != shiporder.SiteID)
				{
					throw new PXException(Messages.ShipViaNotApplicableToShipment, Document.Cache.GetValueExt<SOShipment.siteID>(shiporder));
				}
			}

			if (shiporder.ShippedViaCarrier != true)
			{
				ICarrierService cs = CarrierMaint.CreateCarrierService(this, shiporder.ShipVia);
				CarrierRequest cr = CarrierRatesExt.BuildRequest(shiporder);
				if (cr.Packages.Count > 0)
				{
					CarrierResult<ShipResult> result = cs.Ship(cr);

					if (result != null)
					{
						StringBuilder sb = new StringBuilder();
						foreach (Message message in result.Messages)
						{
							sb.AppendFormat("{0}:{1} ", message.Code, message.Description);
						}

						if (result.IsSuccess)
						{
							using (PXTransactionScope ts = new PXTransactionScope())
							{
								PXTransactionScope.SetSuppressWorkflow(true);
								//re-read document, do not use passed object because it contains fills from Automation that will be committed even 
								//if shipment confirmation will fail later.
								Document.Current = Document.Search<SOShipment.shipmentNbr>(shiporder.ShipmentNbr);
								SOShipment copy = PXCache<SOShipment>.CreateCopy(Document.Current);

								decimal freightCost = 0;

								if (shiporder.UseCustomerAccount != true && (shiporder.GroundCollect != true || !this.CanUseGroundCollect(shiporder)))
								{
									freightCost = ConvertAmtToBaseCury(result.Result.Cost.Currency, arsetup.Current.DefaultRateTypeID, shiporder.ShipDate.Value, result.Result.Cost.Amount);
								}

								copy.FreightCost = freightCost;
								CM.PXCurrencyAttribute.CuryConvCury<SOShipment.curyFreightCost>(Document.Cache, copy);

								if (copy.OverrideFreightAmount != true)
								{
									PXResultset<SOShipLine> res = Transactions.Select();
									FreightCalculator fc = CreateFreightCalculator();
									fc.ApplyFreightTerms<SOShipment, SOShipment.curyFreightAmt>(Document.Cache, copy, res.Count);
								}

								copy.ShippedViaCarrier = true;

								UploadFileMaintenance upload = PXGraph.CreateInstance<UploadFileMaintenance>();

								if (result.Result.Image != null)
								{
									string fileName = string.Format("High Value Report.{0}", result.Result.Format);
									FileInfo file = new FileInfo(fileName, null, result.Result.Image);
									try
									{
									upload.SaveFile(file, FileExistsAction.CreateVersion);
									}
									catch (PXNotSupportedFileTypeException exc)
									{
										throw new PXException(exc, Messages.NotSupportedFileTypeFromCarrier, result.Result.Format);
									}
									PXNoteAttribute.SetFileNotes(Document.Cache, copy, file.UID.Value);
								}
								Document.Update(copy);

								foreach (PackageData pd in result.Result.Data)
								{
									SOPackageDetailEx sdp = PXSelect<SOPackageDetailEx,
										Where<SOPackageDetailEx.shipmentNbr, Equal<Required<SOShipment.shipmentNbr>>,
										And<SOPackageDetailEx.lineNbr, Equal<Required<SOPackageDetailEx.lineNbr>>>>>.Select(this, shiporder.ShipmentNbr, pd.RefNbr);

									if (sdp != null)
									{
										if (pd.Image != null)
										{
											string fileName = string.Format("Label #{0}.{1}", pd.TrackingNumber, pd.Format);
											FileInfo file = new FileInfo(fileName, null, pd.Image);
											try
											{
												upload.SaveFile(file);
											}
											catch (PXNotSupportedFileTypeException exc)
											{
												throw new PXException(exc, Messages.NotSupportedFileTypeFromCarrier, pd.Format);
											}
											PXNoteAttribute.SetFileNotes(Packages.Cache, sdp, file.UID.Value);

											var pluginMethod = PXSelectorAttribute.Select<Carrier.pluginMethod>(this.carrier.Cache, carrier) as PX.Objects.CS.CarrierMethodSelectorAttribute.CarrierPluginMethod;
											string serviceMethod = $"{carrier.PluginMethod} - {pluginMethod?.Description}";
											if (serviceMethod.Length > CarrierLabelHistory.serviceMethod.Length)
											{
												serviceMethod = serviceMethod.Substring(0, CarrierLabelHistory.serviceMethod.Length);
											}
											decimal rateAmount = ConvertAmtToBaseCury(result.Result.Cost.Currency, arsetup.Current.DefaultRateTypeID, shiporder.ShipDate.Value, pd.RateAmount);

											LabelHistory.Insert(new CarrierLabelHistory()
											{
												ShipmentNbr = shiporder.ShipmentNbr,
												LineNbr = pd.RefNbr,
												PluginTypeName = plugin?.PluginTypeName,
												ServiceMethod = serviceMethod,
												RateAmount = rateAmount
											});
										}
										sdp.TrackNumber = pd.TrackingNumber;
										sdp.TrackUrl = pd.TrackingUrl;
										sdp.TrackData = pd.TrackingData;
										Packages.Update(sdp);
									}
								}

								this.Save.Press();
								ts.Complete();
							}
							Document.Cache.RestoreCopy(shiporder, Document.Current);

							//show warnings:
							if (result.Messages.Count > 0)
							{
								Document.Cache.RaiseExceptionHandling<SOShipment.curyFreightCost>(shiporder, shiporder.CuryFreightCost,
									new PXSetPropertyException(sb.ToString(), PXErrorLevel.Warning));

								PXTrace.WriteWarning(sb.ToString());
							}

						}
						else
						{
							if (!string.IsNullOrEmpty(result.RequestData))
								PXTrace.WriteError(result.RequestData);

							Document.Cache.RaiseExceptionHandling<SOShipment.curyFreightCost>(shiporder, shiporder.CuryFreightCost,
									new PXSetPropertyException(Messages.CarrierServiceError, PXErrorLevel.Error, sb.ToString()));

							throw new PXException(Messages.CarrierServiceError, sb.ToString());
						}

					}
				}
			}
		}

		public virtual void GetReturnLabels(SOShipment shiporder)
		{
			if (IsWithLabels(shiporder.ShipVia))
			{
				ICarrierService cs = CarrierMaint.CreateCarrierService(this, shiporder.ShipVia);
				CarrierRequest cr = CarrierRatesExt.BuildRequest(shiporder);

				var results = cs.GetType().FullName.IsIn("PX.FedExCarrier.FedExCarrier", "PX.UpsCarrier.UpsCarrier")
					? cr.Packages
						.ToArray()
						.Select(package =>
						{
									cr.Packages = new List<CarrierBox>() {package};
									return cs.Return(cr);
								})
						.ToArray()
					: new[] {cs.Return(cr)};

				StringBuilder warningSB = new StringBuilder();
				StringBuilder errorSB = new StringBuilder();

				foreach (var result in results.WhereNotNull())
				{
						if (result.IsSuccess)
						{
						var packagesRefNbrs = result.Result.Data.Select(t => t.RefNbr).ToHashSet();
							foreach (SOPackageDetail pd in PXSelect<SOPackageDetail, Where<SOPackageDetail.shipmentNbr, Equal<Required<SOShipment.shipmentNbr>>>>.Select(this, shiporder.ShipmentNbr))
							{
							if (packagesRefNbrs.Contains(pd.LineNbr.Value))
								{
								foreach (NoteDoc nd in PXSelect<NoteDoc, Where<NoteDoc.noteID, Equal<Required<NoteDoc.noteID>>>>.Select(this, pd.NoteID))
									UploadFileMaintenance.DeleteFile(nd.FileID);
								}
							}

							using (PXTransactionScope ts = new PXTransactionScope())
							{
								PXTransactionScope.SetSuppressWorkflow(true);
								UploadFileMaintenance upload = PXGraph.CreateInstance<UploadFileMaintenance>();

								foreach (PackageData pd in result.Result.Data)
								{
								SOPackageDetailEx sdp =
									PXSelect<SOPackageDetailEx,
										Where<SOPackageDetailEx.shipmentNbr, Equal<Required<SOShipment.shipmentNbr>>,
												And<SOPackageDetailEx.lineNbr, Equal<Required<SOPackageDetailEx.lineNbr>>>>>
										.Select(this, shiporder.ShipmentNbr, pd.RefNbr);
									if (sdp != null)
									{
										string fileName = string.Format("ReturnLabel #{0}.{1}", pd.TrackingNumber, pd.Format);
										FileInfo file = new FileInfo(fileName, null, pd.Image);
										upload.SaveFile(file);

										sdp.TrackNumber = pd.TrackingNumber;
										sdp.TrackUrl = pd.TrackingUrl;
										sdp.TrackData = pd.TrackingData;
										PXNoteAttribute.SetFileNotes(Packages.Cache, sdp, file.UID.Value);
										Packages.Update(sdp);

										Carrier carrier = Carrier.PK.Find(this, shiporder.ShipVia);
										CarrierPlugin plugin = CarrierPlugin.PK.Find(this, carrier.CarrierPluginID);

										var pluginMethod = PXSelectorAttribute.Select<Carrier.pluginMethod>(this.carrier.Cache, carrier) as PX.Objects.CS.CarrierMethodSelectorAttribute.CarrierPluginMethod;
										string serviceMethod = $"{carrier.PluginMethod} - {pluginMethod?.Description}";
										if (serviceMethod.Length > CarrierLabelHistory.serviceMethod.Length)
										{
											serviceMethod = serviceMethod.Substring(0, CarrierLabelHistory.serviceMethod.Length);
										}
										decimal rateAmount = ConvertAmtToBaseCury(result.Result.Cost.Currency, arsetup.Current.DefaultRateTypeID, shiporder.ShipDate.Value, pd.RateAmount);

										LabelHistory.Insert(new CarrierLabelHistory()
										{
											ShipmentNbr = shiporder.ShipmentNbr,
											LineNbr = pd.RefNbr,
											PluginTypeName = plugin?.PluginTypeName,
											ServiceMethod = serviceMethod,
											RateAmount = rateAmount
										});
									}
								}

								this.Actions.PressSave();
								ts.Complete();
							}

						foreach (Message message in result.Messages)
							warningSB.AppendFormat("{0}:{1} ", message.Code, message.Description);
					}
					else
							{
						foreach (Message message in result.Messages)
							errorSB.AppendFormat("{0}:{1} ", message.Code, message.Description);
							}
						}

				if (errorSB.Length > 0 && warningSB.Length > 0)
						{
					string msg = string.Format("Errors: {0}" + Environment.NewLine + "Warnings: {1}", errorSB.ToString(), warningSB.ToString());
					Document.Cache.RaiseExceptionHandling<SOShipment.curyFreightCost>(
						shiporder,
						shiporder.CuryFreightCost,
						new PXSetPropertyException(Messages.CarrierServiceError, PXErrorLevel.Error, msg));

					throw new PXException(Messages.CarrierServiceError, msg);
						}
				else if (errorSB.Length > 0)
				{
					Document.Cache.RaiseExceptionHandling<SOShipment.curyFreightCost>(
						shiporder,
						shiporder.CuryFreightCost,
						new PXSetPropertyException(Messages.CarrierServiceError, PXErrorLevel.Error, errorSB.ToString()));

					throw new PXException(Messages.CarrierServiceError, errorSB.ToString());
					}
				else if (warningSB.Length > 0)
				{
					Document.Cache.RaiseExceptionHandling<SOShipment.curyFreightCost>(
						shiporder,
						shiporder.CuryFreightCost,
						new PXSetPropertyException(warningSB.ToString(), PXErrorLevel.Warning));
				}
			}
		}

		protected virtual FreightCalculator CreateFreightCalculator()
		{
			return new FreightCalculator(this);
		}

		public virtual void CancelPackages(SOShipment shiporder)
		{
			if (shiporder.ShippedViaCarrier == true && IsWithLabels(shiporder.ShipVia))
			{
                SOShipment currentShipment = Document.Search<SOShipment.shipmentNbr>(shiporder.ShipmentNbr);

                ICarrierService cs = CarrierMaint.CreateCarrierService(this, currentShipment.ShipVia);

				SOPackageDetailEx sdp = PXSelect<SOPackageDetailEx,
					Where<SOPackageDetailEx.shipmentNbr, Equal<Required<SOShipment.shipmentNbr>>>>.SelectWindowed(this, 0, 1, currentShipment.ShipmentNbr);

				if (sdp != null && !string.IsNullOrEmpty(sdp.TrackNumber))
				{
					CarrierResult<string> result = cs.Cancel(sdp.TrackNumber, sdp.TrackData);

					if (result != null)
					{
						StringBuilder sb = new StringBuilder();
						foreach (Message message in result.Messages)
						{
							sb.AppendFormat("{0}:{1} ", message.Code, message.Description);
						}

						//Clear Tracking numbers no matter where the call to the carrier were successfull or not

						foreach (SOPackageDetailEx pd in PXSelect<SOPackageDetailEx, Where<SOPackageDetailEx.shipmentNbr, Equal<Required<SOShipment.shipmentNbr>>>>.Select(this, currentShipment.ShipmentNbr))
						{
							pd.Confirmed = false;
							pd.TrackNumber = null;
							pd.TrackUrl = null;
							Packages.Update(pd);

							foreach (NoteDoc nd in PXSelect<NoteDoc, Where<NoteDoc.noteID, Equal<Required<NoteDoc.noteID>>>>.Select(this, pd.NoteID))
							{
								UploadFileMaintenance.DeleteFile(nd.FileID);
							}
						}

                        currentShipment.CuryFreightCost = 0;
						if (currentShipment.OverrideFreightAmount != true)
						{
							currentShipment.CuryFreightAmt = 0;
						}
                        currentShipment.ShippedViaCarrier = false;
						Document.Update(currentShipment);
						Document.Cache.RestoreCopy(shiporder, Document.Current);

						this.Save.Press();

						//Log errors if any: (Log Errors/Warnings to Trace do not return them - In processing warning are displayed as errors (( )
						//CancelPackages should not throw Exceptions since CorrectShipment follows it and must be executed.
						if (!result.IsSuccess)
						{
							//Document.Cache.RaiseExceptionHandling<SOPackageDetail.trackNumber>(shiporder, shiporder.CuryFreightCost,
							//        new PXSetPropertyException(Messages.CarrierServiceError, PXErrorLevel.Error, sb.ToString()));

							//throw new PXException(Messages.CarrierServiceError, sb.ToString());

							PXTrace.WriteWarning("Tracking Numbers and Labels for the shipment was succesfully cleared but Carrier Void Service Returned Error: " + sb.ToString());
						}
						else
						{
							//show warnings:
							if (result.Messages.Count > 0)
							{
								//Document.Cache.RaiseExceptionHandling<SOPackageDetail.trackNumber>(shiporder, shiporder.CuryFreightCost,
								//    new PXSetPropertyException(sb.ToString(), PXErrorLevel.Warning));

								PXTrace.WriteWarning("Tracking Numbers and Labels for the shipment was succesfully cleared but Carrier Void Service Returned Warnings: " + sb.ToString());
							}
						}
					}
				}
			}
		}

		protected virtual void PrintPickList(List<SOShipment> list)
		{
			PrintPickList(list, null);
		}

		protected virtual void PrintPickList(List<SOShipment> list, PXAdapter adapter)
		{
			if (list.Count == 0) return;
			Document.Current = list[0];
			int? branchID;
			using (new PXReadBranchRestrictedScope())
			{
				GL.Branch company = Company.Select();
				branchID = company.BranchID;
			}

			PXReportRequiredException ex = null;
			foreach (SOShipment order in list)
			{
				order.PickListPrinted = true;
				Document.Update(order);

				if (order.Hold == true)
					this.releaseFromHold.PressWithSuppressedWorkflowPersist();
			}

			PXRowPersisted shipmentPersisted = (sender, eventArgs) =>
			{
				if (eventArgs != null && eventArgs.Row != null && eventArgs.TranStatus == PXTranStatus.Completed)
				{
					var shipment = (SOShipment)eventArgs.Row;

					if (shipment.PickListPrinted == true)
					{
						Dictionary<string, string> parameters = new Dictionary<string, string>();
						parameters["SOShipment.ShipmentNbr"] = shipment.ShipmentNbr;
						object cstmr = PXSelectorAttribute.Select<SOOrder.customerID>(Document.Cache, shipment);
						string actualReportID = new NotificationUtility(this).SearchReport(SONotificationSource.Customer, cstmr, SOReports.PrintPickList, branchID);
						ex = PXReportRequiredException.CombineReport(ex, actualReportID, parameters);
						ex.Mode = PXBaseRedirectException.WindowMode.New;
					}
				}
			};

			RowPersisted.AddHandler<SOShipment>(shipmentPersisted);

			try
			{
				this.Save.Press();
			}
			finally
			{
				RowPersisted.RemoveHandler<SOShipment>(shipmentPersisted);
			}

			if (ex != null)
			{
				if (PXAccess.FeatureInstalled<FeaturesSet.deviceHub>())
					SMPrintJobMaint.CreatePrintJobGroup(
						adapter,
						new NotificationUtility(this).SearchPrinter,
						SONotificationSource.Customer,
						SOReports.PrintPickList,
						Accessinfo.BranchID, ex,
						ShipmentActions.Messages.PrintPickList);

				throw ex;
			}
		}

		public virtual void PrintCarrierLabels()
		{
			if (Document.Current == null)
				return;

			if (Document.Current.LabelsPrinted == true)
			{
				WebDialogResult result = Document.View.Ask(Document.Current, GL.Messages.Confirmation, Messages.ReprintLabels, MessageButtons.YesNo, MessageIcon.Question);
				if (result != WebDialogResult.Yes)
				{
					return;
				}
				else
				{
					PXTrace.WriteInformation("User Forced Labels Reprint for Shipment {0}", Document.Current.ShipmentNbr);
				}
			}

			PrintCarrierLabels(new List<SOShipment> { Document.Current }, null);
		}

		public virtual void PrintCarrierLabels(List<SOShipment> list)
		{
			PrintCarrierLabels(list, null);
		}

		public virtual void PrintCarrierLabels(List<SOShipment> list, PXAdapter adapter)
		{
			Guid? userDefinedPrinterID = SMPrintJobMaint.GetPrintSettings(adapter)?.PrinterID;

			PXReportRequiredException reportRedirect = null;
			var printerToReportsMap = new Dictionary<Guid, ShipmentRelatedReports>();

			var searchPrinter = Func.Memorize(
				(string shipVia) => SearchPrinter(SONotificationSource.Customer, SOReports.PrintLabels, Accessinfo.BranchID, shipVia));

			var notificationUtility = new NotificationUtility(this);
			var searchReport = Func.Memorize(
				(int? customerID) => notificationUtility.SearchReport(SONotificationSource.Customer, Customer.PK.Find(this, customerID), SOReports.PrintLabels, Accessinfo.BranchID));

			using (PXTransactionScope ts = new PXTransactionScope())
			{
				UploadFileMaintenance upload = PXGraph.CreateInstance<UploadFileMaintenance>();

				foreach (SOShipment shiporder in list)
				{
					Guid printerID = userDefinedPrinterID ?? searchPrinter(shiporder.ShipVia ?? string.Empty) ?? Guid.Empty;
					var reports = printerToReportsMap.Ensure(printerID, () => new ShipmentRelatedReports());

					PXResultset<SOPackageDetailEx> packagesResultset = PXSelect<SOPackageDetailEx, Where<SOPackageDetailEx.shipmentNbr, Equal<Required<SOShipment.shipmentNbr>>>>.Select(this, shiporder.ShipmentNbr);
					if (packagesResultset.Count > 0)
					{
						SOPackageDetailEx firstRecord = packagesResultset[0];

						Guid[] uids = PXNoteAttribute.GetFileNotes(Packages.Cache, firstRecord);
						if (uids.Length > 0)
						{
							FileInfo fileInfo = upload.GetFile(uids[0]);

							if (IsThermalPrinter(fileInfo))
							{
								reports.LabelFiles.AddRange(GetLabelFiles(packagesResultset));
							}
							else
							{
								reports.LaserLabels.Add(shiporder.ShipmentNbr);

								string actualReportID = searchReport(shiporder.CustomerID);
								var parameters = new Dictionary<string, string> { ["shipmentNbr"] = shiporder.ShipmentNbr };
								reports.ReportRedirect = PXReportRequiredException.CombineReport(reports.ReportRedirect, actualReportID, parameters);
								reportRedirect = PXReportRequiredException.CombineReport(reportRedirect, actualReportID, parameters);
								reportRedirect.Mode = PXBaseRedirectException.WindowMode.New;
							}
						}
						else
						{
							PXTrace.WriteWarning("No Label files to print for Shipment {0}", shiporder.ShipmentNbr);
						}
					}

					shiporder.LabelsPrinted = true;
					Document.Update(shiporder);
				}

				foreach (var pair in printerToReportsMap)
				{
					ShipmentRelatedReports reports = pair.Value;

					if (reports.LabelFiles.Count > 1 && CanMerge(reports.LabelFiles))
					{
						FileInfo mergedFile = MergeFiles(reports.LabelFiles);
						reports.LabelFiles.Clear();

						if (upload.SaveFile(mergedFile))
							reports.LabelFiles.Add(mergedFile);
						else
							throw new PXException(Messages.FailedToSaveMergedFile);
					} 
				}

				this.Save.Press();
				ts.Complete();
			}

			if (adapter?.MassProcess == true)
			{
				for (var i = 0; i < list.Count; i++)
					PXProcessing<SOShipment>.SetInfo(i, ActionsMessages.RecordProcessed);
			}

			bool canRedirectToFile = printerToReportsMap.Count == 1 && printerToReportsMap.First().Value.LabelFiles.Count == 1;
			PXRedirectToFileException targetFileRedirect = null;

			foreach (var (printerID, reports) in printerToReportsMap)
			{
				if (reports.LabelFiles.Count > 0)
				{
					if (printerID != Guid.Empty && PXAccess.FeatureInstalled<FeaturesSet.deviceHub>())
						foreach (FileInfo file in reports.LabelFiles)
						{
						SMPrintJobMaint.CreatePrintJobForRawFile(
							adapter,
								delegate { return printerID; }, // already found
							SONotificationSource.Customer,
							SOReports.PrintLabels,
							Accessinfo.BranchID,
									new Dictionary<string, string> { { "FILEID", file.UID.ToString() } },
								PXMessages.Localize(ShipmentActions.Messages.PrintLabels));
						}

					if (canRedirectToFile)
					{
						targetFileRedirect = new PXRedirectToFileException(reports.LabelFiles.First().UID, forceDownload: true);
						canRedirectToFile = false;
					}
				}

				if (reports.LaserLabels.Count > 0 && reports.ReportRedirect != null)
				{
					if (printerID != Guid.Empty && PXAccess.FeatureInstalled<FeaturesSet.deviceHub>())
						SMPrintJobMaint.CreatePrintJobGroup(
							adapter,
							delegate { return printerID; }, // already found
							SONotificationSource.Customer,
							SOReports.PrintLabels,
							Accessinfo.BranchID,
							reports.ReportRedirect,
							PXMessages.Localize(ShipmentActions.Messages.PrintLabels));
				}
			}

			if (targetFileRedirect != null && reportRedirect != null)
				return; // can't redirect to both a file and a report

			if (targetFileRedirect != null)
				throw targetFileRedirect;

			if (reportRedirect != null)
				throw reportRedirect;
		}

		private class ShipmentRelatedReports
		{
			public List<string> LaserLabels { get; } = new List<string>();
			public List<FileInfo> LabelFiles { get; } = new List<FileInfo>();
			public PXReportRequiredException ReportRedirect { get; set; } = null;
		}

		protected virtual Guid? SearchPrinter(string source, string reportID, int? branchID, string shipVia)
		{
			NotificationSetupUserOverride userSetup =
				SelectFrom<NotificationSetupUserOverride>
				.InnerJoin<NotificationSetup>.On<NotificationSetupUserOverride.FK.DefaultSetup>
				.Where<NotificationSetupUserOverride.userID.IsEqual<AccessInfo.userID.FromCurrent>
					.And<NotificationSetupUserOverride.active.IsEqual<True>>
					.And<NotificationSetup.active.IsEqual<True>>
					.And<NotificationSetup.sourceCD.IsEqual<@P.AsString>>
					.And<NotificationSetup.reportID.IsEqual<@P.AsString>>
					.And<NotificationSetupUserOverride.shipVia.IsEqual<@P.AsString>>
					.And<NotificationSetup.nBranchID.IsEqual<@P.AsInt>.Or<NotificationSetup.nBranchID.IsNull>>>
				.OrderBy<NotificationSetup.nBranchID.Desc>
				.View.Select(this, source, reportID, shipVia, branchID);
			if (userSetup?.DefaultPrinterID != null)
				return userSetup.DefaultPrinterID;

			if (source != null && reportID != null)
			{
				NotificationSetup setup =
					SelectFrom<NotificationSetup>
					.Where<NotificationSetup.active.IsEqual<True>
						.And<NotificationSetup.sourceCD.IsEqual<@P.AsString>>
						.And<NotificationSetup.reportID.IsEqual<@P.AsString>>
						.And<NotificationSetup.shipVia.IsEqual<@P.AsString>>
						.And<NotificationSetup.nBranchID.IsEqual<@P.AsInt>.Or<NotificationSetup.nBranchID.IsNull>>>
					.OrderBy<NotificationSetup.nBranchID.Desc>
					.View.SelectWindowed(this, 0, 1, source, reportID, shipVia, branchID);
				if (setup?.DefaultPrinterID != null)
					return setup.DefaultPrinterID;
			}

			return new NotificationUtility(this).SearchPrinter(source, reportID, branchID);
		}

		protected virtual bool IsWithLabels(string shipVia)
		{
			Carrier carrier = Carrier.PK.Find(this, shipVia);
			return carrier != null && carrier.IsExternal == true;
		}

		protected virtual bool IsThermalPrinter(FileInfo fileInfo)
		{
			if (System.IO.Path.HasExtension(fileInfo.Name))
			{
				string extension = System.IO.Path.GetExtension(fileInfo.Name).Substring(1).ToLower();
				if (extension.Length > 2)
				{
					string ext = extension.Substring(0, 3);
					if (ext == "zpl" || ext == "epl" || ext == "dpl" || ext == "spl" || extension == "starpl" || ext == "pdf")
						return true;
					else
						return false;
				}
				else
					return false;
			}
			else
				return false;


		}

		protected virtual IList<FileInfo> GetLabelFiles(PXResultset<SOPackageDetailEx> resultset)
		{
			List<FileInfo> list = new List<FileInfo>(resultset.Count);
			UploadFileMaintenance upload = PXGraph.CreateInstance<UploadFileMaintenance>();

			foreach (SOPackageDetail pack in resultset)
			{
				Guid[] ids = PXNoteAttribute.GetFileNotes(Packages.Cache, pack);
				if (ids.Length > 0)
				{
					if (ids.Length > 1)
					{
						PXTrace.WriteWarning("There are more then one file attached to the package. But only first will be processed for Shipment {0}/{1}", Document.Current.ShipmentNbr, pack.LineNbr);
					}


					FileInfo fileInfo = upload.GetFile(ids[0]);

					list.Add(fileInfo);

				}
			}

			return list;
		}

		private readonly string[] mergeExtensions = { "zpl", "epl", "dpl", "spl", "starpl", "pdf" };

		protected virtual bool CanMerge(IList<FileInfo> files)
		{
			string previousExt = null;
			foreach (var file in files)
			{
				string fileExt = System.IO.Path.GetExtension(file.Name).ToLower();
				if (fileExt.StartsWith("."))
					fileExt = fileExt.Substring(1);
				if (string.IsNullOrEmpty(fileExt))
					return false;

				previousExt = previousExt ?? fileExt;
				if (previousExt != fileExt
				  || (!mergeExtensions.Contains(fileExt)
					&& (fileExt.Length <= 2 || !mergeExtensions.Contains(fileExt.Substring(0, 3)))))
					return false;
			}
			return true;
		}

		protected virtual FileInfo MergeFiles(IList<FileInfo> files)
		{
			FileInfo result = null;
			string extension = null;
			PdfSharp.Pdf.PdfDocument mergedPDFLabels = null;

			try
			{
				using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
				{
					foreach (FileInfo file in files)
					{
						string ext = System.IO.Path.GetExtension(file.Name);

						if (extension == null)
						{
							extension = ext;
						}
						else
						{
							if (ext != extension)
								throw new PXException(Messages.CannotMergeFiles);
						}

						if (ext.ToLowerInvariant() == ".pdf")
						{
							mergedPDFLabels = mergedPDFLabels ?? new PdfSharp.Pdf.PdfDocument();
							using (System.IO.MemoryStream mem = new System.IO.MemoryStream(file.BinData))
							{
								using (var pdfPages = PdfReader.Open(mem, PdfDocumentOpenMode.Import))
								{
									foreach (PdfSharp.Pdf.PdfPage page in pdfPages.Pages)
									{
										mergedPDFLabels.AddPage(page);
									}
								}
							}
							mergedPDFLabels.Save(stream, false);
						}
						else
						{
							stream.Write(file.BinData, 0, file.BinData.Length);
						}
					}
					mergedPDFLabels?.Close();
					string fileName = Guid.NewGuid().ToString() + extension;
					result = new FileInfo(fileName, null, stream.ToArray());
				}
				return result;
			}
			finally
			{
				mergedPDFLabels?.Dispose();
			}
		}

		protected virtual bool ValidateAvailablePackages()
		{
			if (string.IsNullOrEmpty(Document.Current.ShipVia))
				return false;

			var boxes = CreatePackageEngine()
				.GetBoxesByCarrierID(Document.Current.ShipVia)
				.Select(b => b.BoxID)
				.ToHashSet();

			foreach (SOPackageDetail package in Packages.Select())
			{
				if (!boxes.Contains(package.BoxID))
					return false;
			}

			return true;
		}

		public override void Persist()
		{
			foreach (SOShipLine line in Transactions.Cache.Deleted
				.Concat_(Transactions.Cache.Updated)
				.Concat_(Transactions.Cache.Inserted))
			{
				this.SyncUnassigned(line);
			}

			base.Persist();
		}

		public virtual void SyncUnassigned(SOShipLine line)
			{
			if (line.IsUnassigned != true && line.UnassignedQty == 0m || line.Operation != SOOperation.Issue)
				return;

			var item = InventoryItem.PK.Find(this, line.InventoryID.Value);
			INLotSerClass lotSerClass = null;
			if (item != null && item.StkItem == true)
			{
				lotSerClass = INLotSerClass.PK.Find(this, item.LotSerClassID);
			}
			if (lotSerClass == null || lotSerClass.IsManualAssignRequired != true)
				return;

			bool deleteUnassigned = false;
			bool recreateUnassigned = false;
			List<PXResult<Unassigned.SOShipLineSplit>> unassignedSplitRows = null;
			if (Transactions.Cache.GetStatus(line) == PXEntryStatus.Deleted || line.UnassignedQty == 0m)
			{
				deleteUnassigned = true;
			}
			else if (splits.Cache.Updated.RowCast<SOShipLineSplit>().Any(s => s.LineNbr == line.LineNbr)
				|| splits.Cache.Deleted.RowCast<SOShipLineSplit>().Any(s => s.LineNbr == line.LineNbr)
				|| unassignedSplits.Cache.Updated.RowCast<Unassigned.SOShipLineSplit>().Any(s => s.LineNbr == line.LineNbr)
				|| unassignedSplits.Cache.Deleted.RowCast<Unassigned.SOShipLineSplit>().Any(s => s.LineNbr == line.LineNbr))
			{
				recreateUnassigned = true;
			}
			else
			{
				var insertedSplits = splits.Cache.Inserted.RowCast<SOShipLineSplit>().ToList();
				decimal? insertedSplitsQty = insertedSplits.Sum(s => s.BaseQty ?? 0m);

				unassignedSplitRows = PXSelectJoin<Unassigned.SOShipLineSplit,
					LeftJoin<INLocation, On<INLocation.locationID, Equal<Unassigned.SOShipLineSplit.locationID>>>,
					Where<Unassigned.SOShipLineSplit.shipmentNbr, Equal<Required<Unassigned.SOShipLineSplit.shipmentNbr>>,
						And<Unassigned.SOShipLineSplit.lineNbr, Equal<Required<Unassigned.SOShipLineSplit.lineNbr>>>>,
					OrderBy<Asc<INLocation.pickPriority>>>
					.Select(this, line.ShipmentNbr, line.LineNbr).ToList();
				decimal? unassignedSplitsQty = unassignedSplitRows.Sum(r => ((Unassigned.SOShipLineSplit)r).BaseQty);

				decimal? qtyToReduceUnassigned = unassignedSplitsQty - line.UnassignedQty;
				if (insertedSplitsQty <= qtyToReduceUnassigned)
				{
					var locations = new List<int>();
					var locationsAssignedQty = new Dictionary<int, decimal?>();
					foreach (SOShipLineSplit split in insertedSplits)
					{
						int locationID = split.LocationID ?? -1;
						if (!locationsAssignedQty.ContainsKey(locationID))
						{
							locations.Add(locationID);
							locationsAssignedQty.Add(locationID, 0m);
						}
						locationsAssignedQty[locationID] += split.BaseQty;
					}
					locations.Add(int.MinValue);
					locationsAssignedQty[int.MinValue] = qtyToReduceUnassigned - insertedSplitsQty;

					ApplyAssignedQty(locations, locationsAssignedQty, unassignedSplitRows, true);
					ApplyAssignedQty(locations, locationsAssignedQty, unassignedSplitRows, false);
				}
				else
				{
					recreateUnassigned = true;
				}
			}

			if (deleteUnassigned || recreateUnassigned)
			{
				this.DeleteUnassignedSplits(line, unassignedSplitRows);
			}
			line.IsUnassigned = (line.UnassignedQty != 0m);

			if (recreateUnassigned && line.IsUnassigned == true)
			{
				this.RecreateUnassignedSplits(line, lotSerClass);
			}
		}

		private void ApplyAssignedQty(
			List<int> locations, Dictionary<int, decimal?> locationsAssignedQty,
			List<PXResult<Unassigned.SOShipLineSplit>> unassignedSplitRows,
			bool onlyCoincidentLocation)
		{
			foreach (int locationID in locations)
			{
				decimal? qtyToAssign = locationsAssignedQty[locationID];
				while (qtyToAssign > 0m && unassignedSplitRows.Count > 0)
				{
					var coincidentLocIndexes = unassignedSplitRows
						.SelectIndexesWhere(r => ((Unassigned.SOShipLineSplit)r).LocationID == locationID);
					int? selectedIndex = coincidentLocIndexes.Any() ? coincidentLocIndexes.First()
						: !onlyCoincidentLocation ? unassignedSplitRows.Count - 1 : (int?)null;
					if (!selectedIndex.HasValue)
						break;

					var selectedUnassigned = unassignedSplitRows[selectedIndex.Value];
					var split = (Unassigned.SOShipLineSplit)selectedUnassigned;

					if (qtyToAssign >= split.BaseQty)
					{
						qtyToAssign -= split.BaseQty;
						unassignedSplits.Delete(split);
						unassignedSplitRows.RemoveAt(selectedIndex.Value);
					}
					else
					{
						split.BaseQty -= qtyToAssign;
						split.Qty = INUnitAttribute.ConvertFromBase(unassignedSplits.Cache, split.InventoryID, split.UOM, (decimal)split.BaseQty, INPrecision.QUANTITY);
						qtyToAssign = 0m;
						unassignedSplits.Update(split);
					}
				}
				locationsAssignedQty[locationID] = qtyToAssign;
			}
		}

		public virtual void DeleteUnassignedSplits(SOShipLine line, IEnumerable<PXResult<Unassigned.SOShipLineSplit>> unassignedSplitRows)
		{
			if (unassignedSplitRows == null)
			{
				unassignedSplitRows = PXSelect<Unassigned.SOShipLineSplit,
					Where<Unassigned.SOShipLineSplit.shipmentNbr, Equal<Required<Unassigned.SOShipLineSplit.shipmentNbr>>,
						And<Unassigned.SOShipLineSplit.lineNbr, Equal<Required<Unassigned.SOShipLineSplit.lineNbr>>>>>
					.Select(this, line.ShipmentNbr, line.LineNbr).AsEnumerable();
			}
			foreach (Unassigned.SOShipLineSplit s in unassignedSplitRows)
			{
				unassignedSplits.Cache.Delete(s);
			}
		}

		public virtual void RecreateUnassignedSplits(SOShipLine line, INLotSerClass lotSerClass)
		{
			Transactions.Current = line;

			if (lotSerClass.LotSerAssign == INLotSerAssign.WhenReceived)
			{
				SOLineSplit origSplit = PXSelectReadonly<SOLineSplit,
					Where<SOLineSplit.orderType, Equal<Required<SOLineSplit.orderType>>,
						And<SOLineSplit.orderNbr, Equal<Required<SOLineSplit.orderNbr>>,
						And<SOLineSplit.lineNbr, Equal<Required<SOLineSplit.lineNbr>>,
						And<SOLineSplit.splitLineNbr, Equal<Required<SOLineSplit.splitLineNbr>>>>>>>
					.Select(this, line.OrigOrderType, line.OrigOrderNbr, line.OrigLineNbr, line.OrigSplitLineNbr);

				if (!string.IsNullOrEmpty(origSplit?.LotSerialNbr))
				{
					CreateSplitsForAvailableLots(line.UnassignedQty, line.OrigPlanType, origSplit?.LotSerialNbr, line, lotSerClass);
					return;
				}
			}

			CreateSplitsForAvailableNonLots(line.UnassignedQty, line.OrigPlanType, line, lotSerClass);
		}

		private decimal ConvertAmtToBaseCury(string from, string rateType, DateTime effectiveDate, decimal amount)
		{
			decimal result = amount;

			using (ReadOnlyScope rs = new ReadOnlyScope(DummyCuryInfo.Cache))
			{
				CurrencyInfo ci = new CurrencyInfo();
				ci.CuryRateTypeID = rateType;
				ci.CuryID = from;
				ci = (CurrencyInfo)DummyCuryInfo.Cache.Insert(ci);
				ci.SetCuryEffDate(DummyCuryInfo.Cache, effectiveDate);
				DummyCuryInfo.Cache.Update(ci);
				PXCurrencyAttribute.CuryConvBase(DummyCuryInfo.Cache, ci, amount, out result);
				DummyCuryInfo.Cache.Delete(ci);
			}

			return result;
		}

		private void UpdateManualFreightCost(SOShipment shipment, SOOrder order, bool linkModified, bool substract = false)
		{
			if (shipment != null && order != null)
			{
				Carrier carrier = Carrier.PK.Find(this, order.ShipVia);
				if (carrier != null && carrier.CalcMethod == CarrierCalcMethod.Manual)
				{
					if (sosetup.Current?.FreightAllocation == FreightAllocationList.FullAmount
						&& (order.ShipmentCntr > 1 || !linkModified))
						return;

					SOShipment shipmentCopy = PXCache<SOShipment>.CreateCopy(shipment);
					decimal origFreightCost = shipment.ShipmentQty > 0m ? (!substract ? (order.CuryFreightCost ?? 0m) : -(order.CuryFreightCost ?? 0m)) : 0m;
					if (sosetup.Current != null && sosetup.Current.FreightAllocation == FreightAllocationList.Prorate && order.OrderQty != null && order.OrderQty > 0)
					{
						origFreightCost = (shipment.ShipmentQty ?? 0m) / (order.OrderQty ?? 1m) * origFreightCost;
					}
					origFreightCost = substract ? (shipmentCopy.CuryFreightCost ?? 0m) - ((shipmentCopy.CuryFreightCost ?? 0m) + origFreightCost) : (shipmentCopy.CuryFreightCost ?? 0m) + origFreightCost;
					decimal curyFreightCost = 0m;
					PXCurrencyAttribute.CuryConvBase<SOOrder.curyInfoID>(soorder.Cache, order, origFreightCost, out curyFreightCost);
					shipmentCopy.CuryFreightCost = curyFreightCost;
					Document.Update(shipmentCopy);
				}
			}
		}

		public virtual decimal GetQtyThreshold(SOShipLineSplit sosplit)
		{
			decimal threshold =
				SelectFrom<SOLine>
				.InnerJoin<SOShipLine>.On<SOShipLine.FK.OrderLine>
				.Where<SOShipLine.shipmentNbr.IsEqual<@P.AsString>
					.And<SOShipLine.lineNbr.IsEqual<@P.AsInt>>>
				.View.Select(this, sosplit.ShipmentNbr, sosplit.LineNbr)
				.TopFirst?.CompleteQtyMax ?? 100m;
			return threshold / 100m;
		}

		public virtual decimal GetMinQtyThreshold(SOShipLineSplit sosplit)
		{
			decimal threshold =
				SelectFrom<SOLine>
				.InnerJoin<SOShipLine>.On<SOShipLine.FK.OrderLine>
				.Where<SOShipLine.shipmentNbr.IsEqual<@P.AsString>
					.And<SOShipLine.lineNbr.IsEqual<@P.AsInt>>>
				.View.Select(this, sosplit.ShipmentNbr, sosplit.LineNbr)
				.TopFirst?.CompleteQtyMin ?? 100m;
			return threshold / 100m;
		}

		protected virtual bool AnySelected<TSelectedField>(PXCache cache)
			where TSelectedField : IBqlField
		{
			return cache.Cached.Cast<IBqlTable>().Any(
				p => (bool?)cache.GetValue<TSelectedField>(p) == true &&
				cache.GetStatus(p).IsNotIn(PXEntryStatus.Deleted, PXEntryStatus.InsertedDeleted));
		}

		protected virtual void ValidateLineType(SOLine line, InventoryItem item, string message)
		{
			if (item.KitItem == true && item.StkItem != true && line.LineType == SOLineType.NonInventory)
			{
				throw new PXException(message, line.LineNbr, line.OrderNbr);
			}
		}

		protected virtual void MarkConfirmed(SOShipment shipment)
		{
			shipment.Confirmed = true;
			shipment.ConfirmedToVerify = false;
			shipment.Status = SOShipmentStatus.Confirmed;
		}

		protected virtual void MarkOpen(SOShipment shipment)
		{
			shipment.Confirmed = false;
			shipment.ConfirmedToVerify = true;
			shipment.Status = SOShipmentStatus.Open;
		}

		protected virtual void MarkCompleted(SOShipment shipment)
		{
			shipment.Status = SOShipmentStatus.Completed;
		}

		[PXInternalUseOnly]
		protected virtual void SetSuppressWorkflowOnConfirmShipment()
			=> PXTransactionScope.SetSuppressWorkflow(true);

		[PXInternalUseOnly]
		protected virtual void SetSuppressWorkflowOnCorrectShipment()
			=> PXTransactionScope.SetSuppressWorkflow(true);

		[PXInternalUseOnly]
		protected virtual void SetSuppressWorkflowOnUpdateIN()
			=> PXTransactionScope.SetSuppressWorkflow(true);

		public class LineShipment : IEnumerable<SOShipLine>, ICollection<SOShipLine>
		{
			private List<SOShipLine> _List = new List<SOShipLine>();
			public bool AnyDeleted = false;

			#region Ctor

			#region Implementation

		}

		private class ShipmentSchedule : IComparable<ShipmentSchedule>
		{
			private int sortOrder;

			public ShipmentSchedule(PXResult<SOShipmentPlan, SOLineSplit, SOLine, InventoryItem, INLotSerClass, INSite, SOShipLine> result, SOShipLine shipLine)
			{
				this.sortOrder = ((SOLine)result).SortOrder.GetValueOrDefault(1000);
				this.Result = result;
				this.ShipLine = shipLine;
			}

			public PXResult<SOShipmentPlan, SOLineSplit, SOLine, InventoryItem, INLotSerClass, INSite, SOShipLine> Result { get; private set; }
			public SOShipLine ShipLine;

			public int CompareTo(ShipmentSchedule other)
			{
				return sortOrder.CompareTo(other.sortOrder);
			}
		}

		public class OrigSOLineSplitSet : HashSet<SOShipLine>
		{
			public class SplitComparer : IEqualityComparer<SOShipLine>
			{
				public bool Equals(SOShipLine a, SOShipLine b)
				{
					return a.OrigOrderType == b.OrigOrderType && a.OrigOrderNbr == b.OrigOrderNbr
						&& a.OrigLineNbr == b.OrigLineNbr && a.OrigSplitLineNbr == b.OrigSplitLineNbr;
				}

				public int GetHashCode(SOShipLine a)
				{
					unchecked
					{
						int hash = 17;
						hash = hash * 23 + a.OrigOrderType?.GetHashCode() ?? 0;
						hash = hash * 23 + a.OrigOrderNbr?.GetHashCode() ?? 0;
						hash = hash * 23 + a.OrigLineNbr.GetHashCode();
						hash = hash * 23 + a.OrigSplitLineNbr.GetHashCode();
						return hash;
					}
				}
			}

			private SOShipLine _shipLine = new SOShipLine();

			public OrigSOLineSplitSet()
				: base(new SplitComparer())
			{
			}

			public bool Contains(SOLineSplit sls)
			{
				_shipLine.OrigOrderType = sls.OrderType;
				_shipLine.OrigOrderNbr = sls.OrderNbr;
				_shipLine.OrigLineNbr = sls.LineNbr;
				_shipLine.OrigSplitLineNbr = sls.SplitLineNbr;
				return this.Contains(_shipLine);
			}
		}

		#region Well-known extension

		#region Address Lookup Extension

	}
