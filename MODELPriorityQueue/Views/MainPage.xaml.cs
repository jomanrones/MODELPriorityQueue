﻿using MODELPriorityQueue.Modals;
using MODELPriorityQueue.Models;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MODELPriorityQueue.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private async void AddJobButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddJobDialog();
            var result =  await dialog.ShowAsync();
            if (result == ContentDialogResult.Secondary)
            {
                await ViewModel.UpdateDailyStatistic(ViewModel.Jobs.Count + 1);
            }
            await ViewModel.LoadScreenData();
        }

        private async void ViewStatsButton_Click(object sender, RoutedEventArgs e)
        {
            await new StatsDialog().ShowAsync();
        }

        private async void PaymentButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedJob != null)
            {
                await new GenerateBillDialog(ViewModel.SelectedJob).ShowAsync();
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.SaveJob();
        }

        private async void CompletedButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.MarkCompleted();
            await ViewModel.UpdateDailyStatistic(ViewModel.Jobs.Count);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NavigateToSettingsPage();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadScreenData();
        }

        private async void DeleteJobButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.DeleteJob();
            await ViewModel.UpdateDailyStatistic(ViewModel.Jobs.Count);
        }

        //QUEUE STUFF
        private TypedEventHandler<ListViewBase, ContainerContentChangingEventArgs> del;

        private void JobsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Nothing
        }

        async void JobsList_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            QueueJob queueJob = args.ItemContainer.ContentTemplateRoot as QueueJob;
            if(args.InRecycleQueue == true)
            {
                queueJob.ClearData();
            }
            else if(args.Phase == 0)
            {
                //This is where the queueJob gets passed the Job object
                queueJob?.ShowPlaceholder(args.Item as Job);
                args.RegisterUpdateCallback(ContainerContentChangingDelegate);
            }
            else if(args.Phase == 1)
            {
                queueJob?.ShowTitle();
                args.RegisterUpdateCallback(ContainerContentChangingDelegate);
            }
            else if(args.Phase == 2)
            {
                await ViewModel.UpdateQueueOrder();
                queueJob?.ShowCategory();
                queueJob?.ShowImage();
            }

            args.Handled = true;

        }

        private TypedEventHandler<ListViewBase, ContainerContentChangingEventArgs> ContainerContentChangingDelegate
        {
            get
            {
                if (del == null)
                {
                    del = new TypedEventHandler<ListViewBase, ContainerContentChangingEventArgs>(JobsList_ContainerContentChanging);
                }
                return del;
            }
        }

        private async void AssignTechnician_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.AssignTechnician();
        }
    }
}
