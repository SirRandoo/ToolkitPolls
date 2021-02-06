# ToolkitPolls

A common framework for creating, displaying, and running polls within the Toolkit family.

# Implementing

Once you've depended on this assembly, you can schedule polls through the `Coordinator`
game component, or the convenience method `ToolkitPolls.SchedulePoll`. Polls can be created
manually through `IPoll` and `IChoice` subclasses, or through the `PollBuilder` class.

Once a poll has been scheduled, the library will run it when the current poll, if any, has
concluded.
