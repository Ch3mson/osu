﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using OpenTK;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using System;
using osu.Game.Graphics;
using osu.Framework.Allocation;

namespace osu.Game.Screens.Play
{
        public class SongProgress : OverlayContainer
    {
        private const int progress_height = 5;

        protected override bool HideOnEscape => false;

        private static readonly Vector2 handle_size = new Vector2(14, 25);

        private const float transition_duration = 200;

        private readonly SongProgressBar bar;
        private readonly SongProgressGraph graph;

        public Action<double> OnSeek;

        private double progress;
        public double Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                updateProgress();
            }
        }

        public int[] Values
        {
            get { return graph.Values; }
            set { graph.Values = value; }
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            graph.FillColour = bar.FillColour = colours.BlueLighter;
        }

        public SongProgress()
        {
            RelativeSizeAxes = Axes.X;
            Height = progress_height + SongProgressGraph.Column.HEIGHT + handle_size.Y;
            Y = progress_height;

            Children = new Drawable[]
            {
                graph = new SongProgressGraph
                {
                    RelativeSizeAxes = Axes.X,
                    Origin = Anchor.BottomLeft,
                    Anchor = Anchor.BottomLeft,
                    Height = SongProgressGraph.Column.HEIGHT,
                    Margin = new MarginPadding { Bottom = progress_height },
                },
                bar = new SongProgressBar(progress_height, SongProgressGraph.Column.HEIGHT, handle_size)
                {
                    Alpha = 0,
                    Anchor = Anchor.BottomLeft,
                    Origin =  Anchor.BottomLeft,
                    SeekRequested = delegate (float position)
                    {
                        OnSeek?.Invoke(position);
                    },
                },
            };
        }

        protected override void LoadComplete()
        {
            State = Visibility.Visible;
        }

        private void updateProgress()
        {
            bar.UpdatePosition((float)progress);
            graph.Progress = (int)(graph.ColumnCount * progress);
        }

        private bool barVisible;

        public void ToggleBar()
        {
            barVisible = !barVisible;
            updateBarVisibility();
        }

        private void updateBarVisibility()
        {
            bar.FadeTo(barVisible ? 1 : 0, transition_duration, EasingTypes.In);
            MoveTo(new Vector2(0, barVisible ? 0 : progress_height), transition_duration, EasingTypes.In);
        }

        protected override void PopIn()
        {
            updateBarVisibility();
            FadeIn(500, EasingTypes.OutQuint);
        }

        protected override void PopOut()
        {
            FadeOut(100);
        }

        protected override void Update()
        {
            base.Update();

            updateProgress();
        }
    }
}
