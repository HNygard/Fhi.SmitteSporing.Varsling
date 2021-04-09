export interface PagedList<T> {
	totaltAntall: number;
	sideindeks: number;
	sideantall: number;
	antallSider: number;
	resultater: T[];
}

export function mapPagedList<TSrc, TDst>(srcList: PagedList<TSrc>, mapFn: (src: TSrc) => TDst): PagedList<TDst> {
	return {
		totaltAntall: srcList.totaltAntall,
		sideindeks: srcList.sideindeks,
		sideantall: srcList.sideantall,
		antallSider: srcList.antallSider,
		resultater: srcList.resultater.map(mapFn)
	}
}