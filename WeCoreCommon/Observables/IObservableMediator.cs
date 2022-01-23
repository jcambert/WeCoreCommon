using MediatR;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WeCoreCommon.Behaviours;

namespace WeCoreCommon.Observables;

public interface IObservableMediator : ISender
{
    public IObservable<HandlerResponse> OnOk { get; }
    public IObservable<HandlerResponse> OnError { get; }


}
public sealed class ObservableMediator: IObservableMediator
{
    private readonly IMediator InternalMediator;

    private ISubject<HandlerResponse> _onOkSubject = new Subject<HandlerResponse>();
    private ISubject<HandlerResponse> _onErrorSubject = new Subject<HandlerResponse>();
    public IObservable<HandlerResponse> OnOk => _onOkSubject.AsObservable();
    public IObservable<HandlerResponse> OnError => _onErrorSubject.AsObservable();
    public ObservableMediator(IMediator mediator)
    {
        this.InternalMediator = mediator;
    }
    public IAsyncEnumerable<T> CreateStream<T>(IStreamRequest<T> request, CancellationToken cancellationToken = default)
    {
        return InternalMediator.CreateStream<T>(request, cancellationToken);
    }
    //
    // Résumé :
    //     Create a stream via an object request to a stream handler
    //
    // Paramètres :
    //   request:
    //
    //   cancellationToken:
    public IAsyncEnumerable<object> CreateStream(object request, CancellationToken cancellationToken = default)
    {
        return InternalMediator.CreateStream(request, cancellationToken);
    }
   
    //
    // Résumé :
    //     Asynchronously send a request to a single handler
    //
    // Paramètres :
    //   request:
    //     Request object
    //
    //   cancellationToken:
    //     Optional cancellation token
    //
    // Paramètres de type :
    //   TResponse:
    //     Response type
    //
    // Retourne :
    //     A task that represents the send operation. The task result contains the handler
    //     response
    public async Task<T> Send<T>(IRequest<T> request, CancellationToken cancellationToken = default)

    {
        T res = await InternalMediator.Send<T>(request, cancellationToken);
        if (res != null && res is HandlerResponse)
        {
            HandlerResponse response = res as HandlerResponse;
            ISubject<HandlerResponse> subject = !response.IsValidResponse ? _onErrorSubject : _onOkSubject;
            subject.OnNext(response);

        }
        return res;
    }
    //
    // Résumé :
    //     Asynchronously send an object request to a single handler via dynamic dispatch
    //
    // Paramètres :
    //   request:
    //     Request object
    //
    //   cancellationToken:
    //     Optional cancellation token
    //
    // Retourne :
    //     A task that represents the send operation. The task result contains the type
    //     erased handler response
    public async Task<object> Send(object request, CancellationToken cancellationToken = default)
    {
        object res = await InternalMediator.Send(request, cancellationToken);
        if (res != null && res is HandlerResponse)
        {
            HandlerResponse response = res as HandlerResponse;
            ISubject<HandlerResponse> subject = !response.IsValidResponse ? _onErrorSubject : _onOkSubject;
            subject.OnNext(response);

        }
        return res;

    }

    


}