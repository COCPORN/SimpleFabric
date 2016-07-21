# SimpleFabric

The purpose of this project is to try to create a _simple_ implementation of an _API_ that approximates Microsoft Service Fabric, allowing you to use a similar _programming model_ without necessarily giving you any benefits of scalability reliability, resilience, delivery guarantees, etc.

The idea is to make a platform where you can start out developing a throwaway project that is a runaway hit, which in turn makes it easy to move (code and data-wise) to a platform that is highly scalable and resilient.

## Setup

Some of this setup will deviate heavily from Microsoft Service Fabric, as the purpose of this implementation is hugely different from original Service Fabric. So this is not stuff you'll find in the official API. It is, however, stuff that happens before you start using the part of the `SimpleFabric` API that is meant to mimic the Service Fabric API.

### Configuring the `ActorProxy`

If you implement your own proxy type or get one from another library, you can add a `ActorProxyCreator`-`Func`:

```
ActorProxy.ActorProxyCreator = 
    () => { return new NetMQActorProxy(); }
```
This needs to be setup before you start running `Create`-calls on the `ActorProxy`.

### Configuring the `StateManager` for the `Actor` 

Similarly, you can configure the `StateManager` for the `Actor`, which defaults to being the `InMemoryActorStateManager`, which is a thin wrapper on a simple dictionary for storing state. This will not be persisted in any way, so it is useful for unit testing and prototyping.

Configure the `StateManager`:

```
Actor.StateManagerCreator = 
    () => { return new AzureTableStorageActorStateManager(); }
```

## Current state

The state of the library is _highly experimental_. I'll start using it for production work for stuff that is _not mission critical_ any minute, but I urge you not to. 

### Priority

- Actors (started)    
	- Actor lifetime (Activation, Deactivation, KeepAlive on requests)
		- Actor activation and deactivation
    - State in Azure Table Storage (started)
    - State stored to local disk (not started)
	- Timers (not started)
	- Events (not started)
    - Network transparency in proxy using NetMQ (not started)
- Stateless Services
- Stateful Services