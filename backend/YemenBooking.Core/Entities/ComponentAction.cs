using System;

namespace YemenBooking.Core.Entities
{
    public class ComponentAction : BaseEntity
    {
        public Guid ComponentId { get; private set; }
        public string ActionType { get; private set; } // Navigate, OpenModal, CallAPI, Share
        public string ActionTrigger { get; private set; } // Click, LongPress, Swipe
        public string ActionTarget { get; private set; } // Screen name, URL, API endpoint
        public string ActionParams { get; private set; } // JSON parameters
        public string Conditions { get; private set; } // JSON conditions
        public bool RequiresAuth { get; private set; }
        public string AnimationType { get; private set; }
        public int Priority { get; private set; }
        
        public HomeScreenComponent Component { get; private set; }

        protected ComponentAction() { }

        public ComponentAction(
            Guid componentId,
            string actionType,
            string actionTrigger,
            string actionTarget,
            int priority = 0)
        {
            Id = Guid.NewGuid();
            ComponentId = componentId;
            ActionType = actionType;
            ActionTrigger = actionTrigger;
            ActionTarget = actionTarget;
            Priority = priority;
            RequiresAuth = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(
            string actionTarget,
            string actionParams,
            string conditions,
            bool requiresAuth,
            string animationType)
        {
            ActionTarget = actionTarget;
            ActionParams = actionParams;
            Conditions = conditions;
            RequiresAuth = requiresAuth;
            AnimationType = animationType;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}