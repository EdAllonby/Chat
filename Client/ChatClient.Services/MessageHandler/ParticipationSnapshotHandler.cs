using System;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="EntitySnapshot{TEntity}" /> the Client received.
    /// </summary>
    internal sealed class ParticipationSnapshotHandler : MessageHandler<EntitySnapshot<Participation>>, IBootstrapper
    {
        public ParticipationSnapshotHandler(IServiceRegistry serviceRegistry) : base(serviceRegistry)
        {
        }

        public event EventHandler<EntityBootstrapEventArgs> EntityBootstrapCompleted;

        protected override void HandleMessage(EntitySnapshot<Participation> message)
        {
            var participationRepository = (IEntityRepository<Participation>) ServiceRegistry.GetService<RepositoryManager>().GetRepository<Participation>();

            foreach (Participation participation in message.Entities)
            {
                participationRepository.AddEntity(participation);
            }

            OnParticipationBootstrapCompleted();
        }

        private void OnParticipationBootstrapCompleted()
        {
            EventHandler<EntityBootstrapEventArgs> handler = EntityBootstrapCompleted;
            handler?.Invoke(this, new EntityBootstrapEventArgs(typeof(Participation)));
        }
    }
}