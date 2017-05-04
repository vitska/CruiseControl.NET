using System;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// The response containing the status of the projects.
    /// </summary>
    [XmlRoot("hostPerformanceResponse")]
    [Serializable]
    public class HostPerformanceResponse
        : Response
    {
        #region Constructors
        /// <summary>
        /// Initialise a new instance of <see cref="HostPerformanceResponse"/>.
        /// </summary>
        public HostPerformanceResponse()
            : base()
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="ProjectStatusResponse"/> from a request.
        /// </summary>
        /// <param name="request">The request to use.</param>
        public HostPerformanceResponse(ServerRequest request)
            : base(request)
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="ProjectStatusResponse"/> from a response.
        /// </summary>
        /// <param name="response">The response to use.</param>
        public HostPerformanceResponse(Response response)
            : base(response)
        {
        }
        #endregion

        #region Public properties
        [XmlAttribute]
        public byte Cpu;
        [XmlAttribute]
        public byte Memory;
        [XmlAttribute]
        public byte Disk;
        #endregion
    }
}
