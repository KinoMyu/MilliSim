using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Runtime.Extensions;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Theater.Extensions;
using OpenMLTD.MilliSim.Theater.Internal;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public class NoteSfxPlayer : Element {

        public NoteSfxPlayer(GameBase game)
            : base(game) {
        }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);

            if (_notes == null) {
                return;
            }

            var theaterDays = Game.AsTheaterDays();

            var notesLayer = theaterDays.FindSingleElement<NotesLayer>();
            if (notesLayer == null) {
                throw new InvalidOperationException();
            }

            var syncTimer = theaterDays.FindSingleElement<SyncTimer>();
            if (syncTimer == null) {
                throw new InvalidOperationException();
            }

            var sfxPaths = Program.Settings.Sfx;
            var player = theaterDays.AudioManager.Sfx;
            var now = syncTimer.CurrentTime.TotalSeconds;

            var noteMetrics = new NoteMetrics {
                GlobalSpeedScale = notesLayer.GlobalSpeedScale
            };

            var states = _noteStates;

            foreach (var note in _notes) {
                var oldState = states[note];
                var newState = NoteAnimationHelper.GetOnStageStatusOf(note, now, noteMetrics);

                if (oldState == newState) {
                    continue;
                }

                switch (note.Type) {
                    case RuntimeNoteType.Tap:
                        if (newState == OnStageStatus.Passed) {
                            player.Play(sfxPaths.Tap.Perfect);
                        }
                        break;
                    case RuntimeNoteType.Flick:
                        if (newState == OnStageStatus.Passed) {
                            player.Play(sfxPaths.Flick.Perfect);
                        }
                        break;
                    case RuntimeNoteType.Hold:
                        if (note.IsHoldStart()) {
                            if (note.FlickDirection != FlickDirection.None) {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.Flick.Perfect);
                                }
                            } else {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.Hold.Perfect);
                                    player.PlayLooped(sfxPaths.HoldHold, note);
                                }
                            }
                        } else if (note.IsHoldEnd()) {
                            if (note.FlickDirection != FlickDirection.None) {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.Flick.Perfect);
                                }
                            } else {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.HoldEnd.Perfect);
                                }
                            }

                            if (newState == OnStageStatus.Passed) {
                                player.StopLooped(FindFirstHold(note));
                            }
                        }
                        break;
                    case RuntimeNoteType.Slide:
                        if (note.IsSlideStart()) {
                            if (note.FlickDirection != FlickDirection.None) {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.Flick.Perfect);
                                }
                            } else {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.Slide.Perfect);
                                    player.PlayLooped(sfxPaths.SlideHold, note);
                                }
                            }
                        } else if (note.IsSlideEnd()) {
                            if (note.FlickDirection != FlickDirection.None) {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.Flick.Perfect);
                                }
                            } else {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.SlideEnd.Perfect);
                                }
                            }

                            if (newState == OnStageStatus.Passed) {
                                player.StopLooped(FindFirstSlide(note));
                            }
                        }
                        break;
                    case RuntimeNoteType.Special:
                        if (newState == OnStageStatus.Passed) {
                            player.Play(sfxPaths.Special.Perfect);
                            var shouts = sfxPaths.Shouts;
                            if (shouts != null && shouts.Length > 0) {
                                var shoutIndex = MathHelper.Random.Next(shouts.Length);
                                player.Play(shouts[shoutIndex]);
                            }
                            player.PlayLooped(sfxPaths.SpecialHold, note);
                        }
                        break;
                    case RuntimeNoteType.SpecialEnd:
                        if (newState == OnStageStatus.Passed) {
                            player.Play(sfxPaths.SpecialEnd);
                            var shouts = sfxPaths.Shouts;
                            if (shouts != null && shouts.Length > 0) {
                                var shoutIndex = MathHelper.Random.Next(shouts.Length);
                                player.Play(shouts[shoutIndex]);
                            }

                            var specialStart = _notes.SingleOrDefault(n => n.Type == RuntimeNoteType.Special);
                            Debug.Assert(specialStart != null, "Wrong score format: there must be only exactly one special note and one special end note, if either of them exists.");
                            player.StopLooped(specialStart);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                states[note] = newState;
            }
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            var theaterDays = Game.AsTheaterDays();
            var scoreLoader = theaterDays.FindSingleElement<ScoreLoader>();

            var score = scoreLoader?.RuntimeScore;
            if (score != null) {
                foreach (var note in score.Notes) {
                    _noteStates.Add(note, OnStageStatus.Incoming);
                }
                _notes = score.Notes;
            }
        }

        private static RuntimeNote FindFirstHold(RuntimeNote note) {
            var firstHold = note;
            do {
                firstHold = firstHold.PrevHold;
            } while (firstHold.PrevHold != null);
            return firstHold;
        }

        private static RuntimeNote FindFirstSlide(RuntimeNote note) {
            var firstSlide = note;
            do {
                firstSlide = firstSlide.PrevSlide;
            } while (firstSlide.PrevSlide != null);
            return firstSlide;
        }

        [CanBeNull]
        private IReadOnlyList<RuntimeNote> _notes;
        private static readonly Dictionary<RuntimeNote, OnStageStatus> _noteStates = new Dictionary<RuntimeNote, OnStageStatus>();

    }
}